
namespace NoThanks;

public class Strategy
{
    public virtual bool decide(Gamestate gamestate)
    {
        return false;
    }
}