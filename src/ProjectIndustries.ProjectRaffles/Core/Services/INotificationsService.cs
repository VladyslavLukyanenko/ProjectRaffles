using System;
using System.Collections.Generic;
using DynamicData;
using DynamicData.Binding;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface INotificationsService
  {
    IObservable<IEnumerable<NotificationViewModel>> Notifications { get; }
    void Add(NotificationViewModel notification);
    void Clear();
  }
}