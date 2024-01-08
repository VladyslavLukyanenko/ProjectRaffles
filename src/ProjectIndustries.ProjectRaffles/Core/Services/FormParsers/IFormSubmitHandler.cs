using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers
{
  public interface IFormSubmitHandler
  {
    ValueTask<FormSubmitResult> SubmitAsync(Uri pageUrl, IEnumerable<Field> fields, CancellationToken ct = default);
  }
}