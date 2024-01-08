using System;
using System.Windows.Threading;

namespace ProjectIndustries.ProjectRaffles.WpfUI.MapBox.MapboxNetWPF
{
    class Relay
    {
        Dispatcher dispatcher;
        Action<string> callback;

        public Relay(Action<string> callback, Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.callback = callback;
        }

        public void notify(string json)
        {
            dispatcher.BeginInvoke((Action)(() =>
            {
                callback(json);
            }));
        }
    }
}
