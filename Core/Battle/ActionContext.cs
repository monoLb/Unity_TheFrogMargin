using System.Collections;
using System.Collections.Generic;

public class ActionContext
{
    public Queue<IEnumerator> subActions = new();

    public void Enqueue(IEnumerator action)
    {
        subActions.Enqueue(action);
    }

    public IEnumerator ExecuteSubActions()
    {
        while (subActions.Count > 0)
        {
            yield return subActions.Dequeue();
        }
    }
}