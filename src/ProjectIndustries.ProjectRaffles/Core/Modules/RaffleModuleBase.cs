using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Apm.Api;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ReactiveUI;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public abstract class RaffleModuleBase : IRaffleModule
  {
    private RaffleStatus _status = RaffleStatus.Created;
    private CompositeDisposable _identitySubscriptions;
    private BehaviorSubject<string> _identity = new BehaviorSubject<string>(null);
    protected IRaffleExecutionContext ExecutionContext;
    private readonly ITracer _tracer = Locator.Current.GetService<ITracer>();

    private ISpan _currentSpan;
    private ITransaction _currentTransaction;

    protected RaffleModuleBase(bool skipFieldsInitialization = false)
    {
      var nameAttr = GetType().GetCustomAttribute<RaffleModuleNameAttribute>();
      if (nameAttr == null)
      {
        throw new InvalidOperationException($"{nameof(RaffleModuleNameAttribute)} is missing on module " +
                                            GetType().Name);
      }

      Name = nameAttr.Name;
      ComputedIdentityChanged = _identity.AsObservable().DistinctUntilChanged();

      if (!skipFieldsInitialization)
      {
        // ReSharper disable once VirtualMemberCallInConstructor
        _ = InitializeDefaultIdentityFields();
      }
    }

    protected virtual async Task InitializeDefaultIdentityFields(CancellationToken ct = default)
    {
      _identitySubscriptions?.Dispose();
      _identitySubscriptions = new CompositeDisposable();
      foreach (var field in AdditionalFields)
      {
        await field.InitializeAsync(Locator.Current, ct);
      }

      IdentityFields = AdditionalFields.Where(f => f?.IsRequired == true);
      var changes = IdentityFields.Select(_ => _.Changed);
      changes.CombineLatest()
        .Throttle(TimeSpan.FromMilliseconds(300))
        .Select(_ => _.Where(f => !f.IsEmpty))
        .Where(_ => _.Any())
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(fields =>
        {
          ComputedIdentity = Name + ":" + string.Join("__", fields.Select(_ => _.SystemName + "=" + _.ValueId));
        })
        .DisposeWith(_identitySubscriptions);
    }

    public virtual Product TargetProduct { get; private set; }

    public string Name { get; }

    public string ComputedIdentity
    {
      get => _identity.Value;
      private set
      {
        if (value == _identity.Value)
        {
          return;
        }

        _identity.OnNext(value);
      }
    }

    protected abstract IModuleHttpClient HttpClient { get; }

    public IObservable<string> ComputedIdentityChanged { get; }

    public void SetFields(IDictionary<string, object> participateArguments)
    {
      participateArguments.Export(AdditionalFields);
    }

    protected virtual void SetFields(IEnumerable<Field> fields)
    {
      var fieldValues = fields.ToDictionary(_ => _.SystemName, _ => _);
      foreach (var field in AdditionalFields)
      {
        if (fieldValues.TryGetValue(field.SystemName, out var source))
        {
          source.CopyTo(field);
        }
      }
    }

    public virtual void InitializeFromPrototype(IRaffleModule module)
    {
      if (GetType() != module.GetType())
      {
        throw new ArgumentException(
          $"Module type must be the same. Provided module '{module.GetType().Name}' for '{GetType().Name}'");
      }

      SetFields(module.AdditionalFields);
    }

    public virtual IEnumerable<Field> IdentityFields { get; set; }
    public abstract IEnumerable<Field> AdditionalFields { get; }

    public RaffleStatus Status
    {
      get => _status;
      protected set
      {
        if (value == _status)
        {
          return;
        }

        _currentSpan?.End();
        _status = value;

        _currentSpan = _currentTransaction?.StartSpan(_status.Name, ApmSpanTypes.ModuleExecutionStep);
        OnStatusChange();
      }
    }

    public event EventHandler StatusChanged;
    public event EventHandler AdditionalFieldsChanged;

    public async Task ExecuteAsync(IRaffleExecutionContext context, CancellationToken ct)
    {
      await _tracer.CaptureTransaction(Name, ApmTxTypes.Modules, async tx =>
      {
        _currentTransaction = tx;
        foreach (var field in AdditionalFields)
        {
          tx.Context.Custom[field.SystemName] = field.Value is string s ? s : field.ValueId;
        }

        try
        {
          await ExecuteAsync(context.Profile, ct);
        }
        catch (Exception exc)
        {
          tx.CaptureException(exc);
          throw;
        }
        finally
        {
          _currentSpan?.End();
        }


        if (Status.Kind == RaffleStatusKind.Failed)
        {
          tx.CaptureError(Status.Name, "ExecutedWithFailure", Array.Empty<StackFrame>());
        }
      });
    }

    protected abstract Task ExecuteAsync(Profile profile, CancellationToken ct);

    public virtual void SetHttpClientBuilder(IHttpClientBuilder builder)
    {
      HttpClient.Initialize(builder);
    }

    public virtual Task InitializeAsync(IRaffleExecutionContext context, CancellationToken ct)
    {
      ExecutionContext = context;
      var hostTasks = AdditionalFields.OfType<IRequiresPreInitialization>()
        .Select(_ => _.PrepareAsync(context, ct));

      // foreach (var field in AdditionalFields.OfType<ISiblingsDependentField>())
      // {
      //   field.Consume(AdditionalFields);
      // }

      return Task.WhenAll(hostTasks);
    }

    public virtual async Task PrepareAsync(CancellationToken ct)
    {
      Status = RaffleStatus.Preparation;
      TargetProduct = await FetchProductAsync(ct);
      Status = RaffleStatus.Ready;
    }

    protected virtual void OnStatusChange()
    {
      StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    protected abstract Task<Product> FetchProductAsync(CancellationToken ct);

    protected virtual async Task OnAdditionalFieldsChanged(CancellationToken ct = default)
    {
      await InitializeDefaultIdentityFields(ct);
      AdditionalFieldsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
      _identitySubscriptions.Dispose();
    }
  }
}