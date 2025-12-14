using System;

public interface INpcMinigame
{
    event Action Solved;
    event Action Failed;

    void Begin();
}
