interface IHitable
{
    void OnHit(Team team);
}

interface ISink{
    void OnSink(Balls ball);
}

public interface IObserver<T>
{
    void OnNotify(object sender, T eventArgs);
    void OnNotifyHit(object sender, T eventArgs);

}