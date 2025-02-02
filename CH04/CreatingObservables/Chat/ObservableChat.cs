﻿using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace CreatingObservables.Chat
{
public class ObservableConnection : ObservableBase<string>
{
    private readonly IChatConnection _chatConnection;

    public ObservableConnection(IChatConnection chatConnection)
    {
        _chatConnection = chatConnection;
    }
   
    protected override IDisposable SubscribeCore(IObserver<string> observer)
    {
        /* In this case, the observable class incorporates an event source, this event source 
         * is assigned with oberver's methods
         */
        Action<string> received = message =>
        {
            observer.OnNext(message);
        };

        Action closed = () =>
        {
            observer.OnCompleted();
        };

        Action<Exception> error = ex =>
        {
            observer.OnError(ex);
        };

        _chatConnection.Received += received;
        _chatConnection.Closed += closed;
        _chatConnection.Error += error;

        /* return type of subscripion is a Disposable, which will do resource release for 
        observable when despose */
        return Disposable.Create(() =>
        {
            _chatConnection.Received -= received;
            _chatConnection.Closed -= closed;
            _chatConnection.Error -= error;
            _chatConnection.Disconnect();
        });
    }
}

}
