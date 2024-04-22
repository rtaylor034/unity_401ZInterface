using System.Collections;
using System.Collections.Generic;
using Token;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using FourZeroOne;
using UnityEngine.InputSystem;
using Perfection;

// very placeholder behavior.
public class GameWorld : MonoBehaviour, FourZeroOne.IInputProvider, FourZeroOne.IOutputProvider
{

    public void Awake()
    {
    }
    public void OnEnable()
    {

    }
    public void OnDisable()
    {
    }
    public async void Start()
    {
        
    }
    public void Update()
    {
        
    }
    ITask<IEnumerable<R>> IInputProvider.ReadMultiSelection<R>(IEnumerable<R> outOf, int count)
    {
        return SelectionLogic(outOf, count);
    }

    async ITask<R> IInputProvider.ReadSelection<R>(IEnumerable<R> outOf) where R : class
    {
        return (await SelectionLogic(outOf, 1))[0];
    }
    private async ITask<List<R>> SelectionLogic<R>(IEnumerable<R> outOf, int count)
    {
        var o = new List<R>(count);
        if (0 >= count) return o;
        var options = new List<(Renderer visual, R data)>();
        int index = 0;
        int _total = 0;

        var defaultColor = new Color(0.4f, 0.4f, 0.4f);
        var defaultSize = new Vector3(0.5f, 0.5f, 0.5f);
        foreach (var (i, v) in outOf.Enumerate())
        {
            _total++;
            var visual = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<Renderer>();
            visual.transform.localScale = defaultSize;
            visual.transform.position = new Vector3(i * 0.8f - 2f, 0, 0);
            visual.material.color = defaultColor;
            options.Add((visual, v));
        }
        count = (count > _total) ? _total : count;
        var resolveOnAllSelected = new ControlledTask.ControlledTask();
        var input = new Generated.TestInput();
        input.Selection.left.performed += _ => __Left();
        input.Selection.right.performed += _ => __Right();
        input.Selection.enter.performed += _ => __Select();
        input.Enable();
        __Hover();

        await resolveOnAllSelected;
        foreach (var (visual, _) in options) Destroy(visual);
        input.Disable();
        input.Dispose();
        Debug.Log($"SELECTED: {new PList<R>() { Elements = o }}");
        return o;

        void __Left()
        {
            options[index].visual.transform.localScale = defaultSize;
            index = (index > 0) ? index - 1 : 0;
            __Hover();
        }
        void __Right()
        {
            options[index].visual.transform.localScale = defaultSize;
            index = (_total > index) ? index + 1 : _total;
            __Hover();
        }
        void __Hover()
        {
            options[index].visual.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Debug.Log($"HOVER: {options[index].data}");
        }
        void __Select()
        {
            var sel = options[index];
            if (o.Remove(sel.data))
            {
                sel.visual.material.color = defaultColor;
            } else
            {
                sel.visual.material.color = new Color(0.4f, 0.8f, 0.4f);
                o.Add(sel.data);
                if (o.Count >= count) resolveOnAllSelected.Resolve();
            }
        }
    }

    ITask IOutputProvider.WriteState(State state)
    {
        return Task.CompletedTask.AsITask();
    }
}
