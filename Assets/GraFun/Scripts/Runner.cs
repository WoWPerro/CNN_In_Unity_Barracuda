using Unity.Barracuda;
using UnityEngine;
using FreeDraw;

public class Runner : MonoBehaviour
{
    [Header("Texture to analize")]
    public Texture2D texture;
    private Texture2D textureToAnalize;

    [Space(10)]
    [Header("CNNS")]
    public NNModel asset_ParagraphsVsDrawings;
    public NNModel asset_RightMargin;
    public NNModel asset_LeftMarginForm;
    //public NNModel asset_LeftMarginSpace;
    public NNModel asset_UpperMargin;
    public NNModel asset_DownMargin;
    // NNModel asset_Direction;
    public NNModel asset_Form;

    private Model model_ParagraphsVSDrawings;
    private Model model_RightMargin;
    private Model model_LeftMarginForm;
    private Model model_LeftMarginSpace;
    private Model model_UpperMargin;
    private Model model_DownMargin;
    private Model model_Direction;
    private Model model_Form;

    [Space(10)]
    [Header("Graph")]
    public GameObject graph;

    //Player Variables
    public float killer = 0.1f;
    public float achiever = 0.1f;
    public float explorer = 0.1f;
    public float socializer = 0.1f;

    //
    private bool paragraph = false;

    void Start()
    {
        Drawable.Pen_Colour = Color.black;
    }

    public void AnalizeImage()
    {
        Resize(texture);
        ParagraphsVsDrawings();
        if(paragraph)
        {
            RightMargin();
            LeftMarginForm();
            LeftMarginSpace();
            UpperMargin();
            DownMargin();
            Direction();
            Form();
            graph.GetComponent<PieGraph>().makeGraph();
            Debug.Log("RESULTADOS, (Killer : " + killer + ") (achiever " + achiever + ") (explorer " + explorer + ") (socializer " + socializer + ")");
        }
        else
        {
            
        }
    }

    /// <summary>
    /// Función que decide si la imagen es un párrafo o texto
    /// Si da negativo, entonces no se ejecutará el análisis
    /// </summary>
    private void ParagraphsVsDrawings() //Paragraphs or Drawings
    {
        IWorker worker;
        model_ParagraphsVSDrawings = ModelLoader.Load(asset_ParagraphsVsDrawings);

        //Hay distintos tipos de tipes en https://docs.unity3d.com/Packages/com.unity.barracuda@1.0/manual/Worker.html
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model_ParagraphsVSDrawings); 

        var channelCount = 1; // you can treat input pixels as 1 (grayscale), 3 (color) or 4 (color with alpha) channels
        Tensor input = new Tensor(textureToAnalize, channelCount);
        worker.Execute(input);
        Tensor Output = worker.PeekOutput();
        var arr = Output.ToReadOnlyArray();
        //Output.Print();
        //Debug.Log(arr[0]);
        float result = arr[0];
        if(Mathf.Abs(0 - result) < Mathf.Abs(1 - result))
        {
            paragraph = false;
            Debug.Log("Esto no es un párrafo");
        }
        else
        {
            paragraph = true;
            Debug.Log("Esto es un párrafo");
        }
        worker.Dispose();
        Output.Dispose();
        input.Dispose();
    }

    private void RightMargin() //Normal Grande Ausente
    {
        IWorker worker;
        model_RightMargin = ModelLoader.Load(asset_RightMargin);

        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model_RightMargin); 

        var channelCount = 1;
        Tensor input = new Tensor(textureToAnalize, channelCount);
        worker.Execute(input);
        Tensor Output = worker.PeekOutput();
        var arr = Output.ToReadOnlyArray();
        var max = Mathf.Max(arr);
        if(arr[0] == max) //Normal
        {
            Debug.Log("RightMarginNormal_Explorer+2");
            explorer += 2;
        }
        else if (arr[1] == max) //Grande
        {
            Debug.Log("RightMarginGrande_Social+1");
            socializer += 1;
        }
        else if (arr[2] == max) //Ausente
        {
            Debug.Log("RightMarginAusente_Social+3, rest+1");
            socializer += 3;
            explorer+=1;
            killer+=1;
            achiever+=1;
        }
        worker.Dispose();
        Output.Dispose();
        input.Dispose();
    }

    private void LeftMarginForm() //zigzag, estrechándose, ebsanchándose, convexo, concavo
    {
        IWorker worker;
        model_LeftMarginForm = ModelLoader.Load(asset_LeftMarginForm);

        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model_LeftMarginForm); 

        var channelCount = 1;
        Tensor input = new Tensor(textureToAnalize, channelCount);
        worker.Execute(input);
        Tensor Output = worker.PeekOutput();
        var arr = Output.ToReadOnlyArray();
        var max = Mathf.Max(arr);
        if(arr[0] == max) //ZigZag
        {
            Debug.Log("LeftMarginZigZag_social+1");
            socializer += 1;
        }
        else if (arr[1] == max) //Estrechandose
        {
            Debug.Log("LeftMarginEstrechandose_achiver+2");
            achiever += 2;
        }
        else if (arr[2] == max) //Ensanchándose
        {
            Debug.Log("LeftMarginEnsanchandose_killer+3, achiever+2, rest+1");
            socializer += 1;
            explorer+=1;
            killer+=3;
            achiever+=2;
        }
        else if (arr[3] == max) //Convexo
        {
            Debug.Log("LeftMarginConvexo_");
        }
        else if (arr[4] == max) //Concavo
        {
            Debug.Log("RightMarginConcavo_");
        }
        
        worker.Dispose();
        Output.Dispose();
        input.Dispose();
    }

    private void LeftMarginSpace()
    {

    }

    private void UpperMargin() //Grande, Normal, Pequeño Ausente
    {
        IWorker worker;
        model_UpperMargin = ModelLoader.Load(asset_UpperMargin);

        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model_UpperMargin); 

        var channelCount = 1;
        Tensor input = new Tensor(textureToAnalize, channelCount);
        worker.Execute(input);
        Tensor Output = worker.PeekOutput();
        var arr = Output.ToReadOnlyArray();
        var max = Mathf.Max(arr);
        if(arr[0] == max) //Grande
        {
            Debug.Log("UpperMarginGrande_Explorer+2, social+1");
            socializer += 1;
            explorer+=2;
        }
        else if (arr[1] == max) //Normal
        {
            Debug.Log("UpperMarginNormal_Explorer+2, rest+1");
            socializer += 1;
            explorer+=2;
            killer+=1;
            achiever+=1;
        }
        else if (arr[2] == max) //Pequeño
        {
            Debug.Log("UpperMarginPequeño_Killer+3, Achiever/Explorer+2, Social+1");
            socializer += 1;
            explorer+=2;
            killer+=3;
            achiever+=2;
        }
        else if (arr[3] == max) //Ausente
        {
            Debug.Log("UpperMarginAusente_Killer+3, Achiever/Explorer+2, Social+1");
            socializer += 1;
            explorer+=2;
            killer+=3;
            achiever+=2;
        }
        worker.Dispose();
        Output.Dispose();
        input.Dispose();
    }

    private void DownMargin() //Pequeño, Normal, Grande, Ausente
    {
        IWorker worker;
        model_DownMargin = ModelLoader.Load(asset_DownMargin);

        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model_DownMargin); 

        var channelCount = 1;
        Tensor input = new Tensor(textureToAnalize, channelCount);
        worker.Execute(input);
        Tensor Output = worker.PeekOutput();
        var arr = Output.ToReadOnlyArray();
        var max = Mathf.Max(arr);
        if(arr[0] == max) //Pequeño
        {
            Debug.Log("DownMarginPequeño_Explorer/Achiever+2, rest+1");
            socializer += 1;
            explorer+=2;
            killer+=1;
            achiever+=2;
        }
        else if (arr[1] == max) //Normal
        {
            Debug.Log("DownMarginNormal_rest+0");
        }
        else if (arr[2] == max) //Grande
        {
            Debug.Log("DownMarginGrande_social+2");
            socializer += 2;
        }
        else if (arr[3] == max) //Ausente
        {
            Debug.Log("DownMarginAusente_killer+3, explorer+2, rest+1");
            socializer += 1;
            explorer+=2;
            killer+=3;
            achiever+=1;
        }
        worker.Dispose();
        Output.Dispose();
        input.Dispose();
    }

    private void Direction()
    {

    }

    private void Form() //Semi, Curva, Angulosa
    {
        IWorker worker;
        model_Form = ModelLoader.Load(asset_Form);

        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model_Form); 

        var channelCount = 1;
        Tensor input = new Tensor(textureToAnalize, channelCount);
        worker.Execute(input);
        Tensor Output = worker.PeekOutput();
        var arr = Output.ToReadOnlyArray();
        var max = Mathf.Max(arr);
        if(arr[0] == max) //Semi
        {
            Debug.Log("Angulosa_Social+1");
            socializer += 1;
        }
        else if (arr[1] == max) //Curva
        {
            Debug.Log("Curva_social+3, achiever+2, rest+1");
            socializer += 3;
            explorer+=1;
            killer+=1;
            achiever+=2;
        }
        else if (arr[2] == max) //Angulosa
        {
            Debug.Log("Angulosa_killer+3, Explorer/Achiever+2, social+1");
            socializer += 1;
            explorer+=2;
            killer+=3;
            achiever+=2;
        }
        worker.Dispose();
        Output.Dispose();
        input.Dispose();
    }

    // private void Resize(Sprite sprite)
    // {
    //     Texture2D tex = sprite.texture;

    //     tex.Resize(310, 438, TextureFormat.RGBA32, false);

    //     textureToAnalize = tex;
    //     //Sprite n_spr = Sprite.Create(tex,new Rect(0, 0, tex.width, tex.height),new Vector2(0.5f, 0.5f), 100.0f);
    // }

    private void Resize(Texture2D tex)
    {
        Texture2D copyTexture = new Texture2D(tex.width, tex.height);
        copyTexture.SetPixels(tex.GetPixels());
        copyTexture.Apply();
        copyTexture.Resize(310, 438, TextureFormat.RGBA32, false);

        textureToAnalize = copyTexture;
        //Sprite n_spr = Sprite.Create(tex,new Rect(0, 0, tex.width, tex.height),new Vector2(0.5f, 0.5f), 100.0f);
    }
}
