namespace Platform.Identity.Abstractions.Contracts;

public interface IActorProvider
{
    ActorContext? GetCurrent();
}