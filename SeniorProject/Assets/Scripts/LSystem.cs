using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class LSystem : MonoBehaviour 
{
    public float growSpeed=20;
    public float leafGrowSpeed = 2;
    public List<Rules> personalizedRules = new List<Rules>();
    public int iterations;
    public float angle;
    public float width;
    public float minLeafLength;
    public float maxLeafLength;
    public float minlength;
    public float maxLength;
    public float variance;
    public bool hasTreeChanged;
    public GameObject tree;
    public Rules currentlyAppliedRules;
    public GameObject treeParent;
    public GameObject branch;
    public GameObject leaf;

    public bool randomTree;
    

	private static LSystem instance;
	public static LSystem Instance {get { return instance; } }


    private const string axiom = "X";


	private Dictionary<char, string> rules = new Dictionary<char, string>();

    private Stack<SavedTransform> transformStack = new Stack<SavedTransform>();

    private Vector3 initialPosition;

    Vector2 boundsMinMaxX;
    Vector2 boundsMinMaxY;
    List<TreeElement> allLines;
    List<Coroutine> animationRoutines;


    private string currentString = "";
    private float[] randomRotations;

	private void Awake()
	{
        instance = this;

        if (randomTree == true)
        {
            iterations = UnityEngine.Random.Range(3,4);
            angle = UnityEngine.Random.Range(5, 120);
            width = UnityEngine.Random.Range(0.02f, 0.07f);
            minLeafLength = UnityEngine.Random.Range(0.05f, 0.1f);
            maxLeafLength = UnityEngine.Random.Range(0.11f, 0.2f);
            minlength = UnityEngine.Random.Range(0.05f, 0.075f);
            maxLength = UnityEngine.Random.Range(0.0751f, 0.16f);
            variance = UnityEngine.Random.Range(1, 10);
        }
            randomRotations = new float[1000];
        for (int i = 0; i < randomRotations.Length; i++)
		{
            randomRotations[i] = Random.Range(-1.0f, 1.0f);
        }

		if(personalizedRules.Count > 0)
		{
            currentlyAppliedRules = personalizedRules[0];
			TranslateRulesToDictionary();
        } 
		else 
		{

        	rules.Add('X', "[F[+FX][*+FX][/+FX]]");
        	rules.Add('F', "FF");
		}

        Generate();
    }

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
            StartGrowthAnimation();
        }
	}

	void StartGrowthAnimation() {
        CancelAnimation();
        animationRoutines = new List<Coroutine>();
        var a = StartCoroutine(AnimateGrowth());
        animationRoutines.Add(a);
    }

	IEnumerator AnimateGrowth() {
        var lines = allLines;
        foreach (var l in lines)
            l.gameObject.SetActive(false);

        float leafWaitTime = 0;

        foreach (var treeElement in lines) {
			if (treeElement.isLeaf) {
                leafWaitTime += Random.Range(.5f, 2);
                var a = StartCoroutine(AnimateLeaf(treeElement.lineRenderer, leafWaitTime));
                animationRoutines.Add(a);
                leafWaitTime += 1 / leafGrowSpeed;
                continue;
            }
			else {
                leafWaitTime = 0;
            }
            float t = 0;
            Vector3 target = treeElement.lineRenderer.GetPosition(1);
            Vector3 start = treeElement.lineRenderer.GetPosition(0);
            treeElement.lineRenderer.SetPosition(1, start);
			treeElement.lineRenderer.gameObject.SetActive(true);

			while(t<1) {
                t += Time.deltaTime * growSpeed;
                treeElement.lineRenderer.SetPosition(1, Vector3.Lerp(start, target, t));
                yield return null;
            }
        }
    }

    IEnumerator AnimateLeaf(LineRenderer leaf, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        float t = 0;
		Vector3 target = leaf.GetPosition(1);
		Vector3 start = leaf.GetPosition(0);
		leaf.SetPosition(1, start);
		leaf.gameObject.SetActive(true);

		while(t<1) {
			t += Time.deltaTime * leafGrowSpeed;
			leaf.SetPosition(1, Vector3.Lerp(start, target, t));
			yield return null;
		}
	}

	void CancelAnimation() {
		if (animationRoutines != null) {
			foreach (var c in animationRoutines) {
                StopCoroutine(c);
            }
		}
	}


	public void Generate()
	{
        CancelAnimation();
        allLines = new List<TreeElement>();
        boundsMinMaxX = new Vector2(float.MaxValue, float.MinValue);
        boundsMinMaxY = new Vector2(float.MaxValue, float.MinValue);

        Destroy(tree);
        tree = Instantiate(treeParent);


        currentString = axiom;

        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < iterations; i++)
		{



            char[] currentStringChars = currentString.ToCharArray();
            for (int j = 0; j < currentStringChars.Length; j++)
			{
                stringBuilder.Append(rules.ContainsKey(currentStringChars[j]) ? rules[currentStringChars[j]] : currentStringChars[j].ToString());
            }

            currentString = stringBuilder.ToString();
            stringBuilder = new StringBuilder();
        }

        for (int k = 0; k < currentString.Length; k++)
        {
            switch (currentString[k])
            {
                case 'F':
                    initialPosition = transform.position;
                    bool isLeaf = false;

                    GameObject currentElement;
                    if (currentString[k + 1] % currentString.Length == 'X' || currentString[k + 3] % currentString.Length == 'F' &&
                    currentString[k + 4] % currentString.Length == 'X')
                    {
                        currentElement = Instantiate(leaf);
                        isLeaf = true;
                    }
                    else
                    {
                        currentElement = Instantiate(branch);
                    }

                    currentElement.transform.SetParent(tree.transform);


                    TreeElement currentTreeElement = currentElement.GetComponent<TreeElement>();


                    currentTreeElement.lineRenderer.SetPosition(0, initialPosition);

					if(isLeaf)
					{
						transform.Translate(Vector3.up * 2f * Random.Range(minLeafLength,maxLeafLength));
					} else
					{
						transform.Translate(Vector3.up * 2f * Random.Range(minlength, maxLength));
					}
                    currentTreeElement.lineRenderer.SetPosition(1, transform.position);
					if(isLeaf)
					{
						currentTreeElement.lineRenderer.startWidth = width * 2f;
                        currentTreeElement.lineRenderer.endWidth = width / 4f;
                        currentTreeElement.isLeaf = true;
                    }
					else
					{
						currentTreeElement.lineRenderer.startWidth = width;
                    	currentTreeElement.lineRenderer.endWidth = width;
					}
                    
                    currentTreeElement.lineRenderer.sharedMaterial = currentTreeElement.lineMaterial;
                    allLines.Add(currentTreeElement);
                    break;

				case 'X':
                    break;
	


				case '+':
					transform.Rotate(Vector3.back * angle * (1 + variance / 100 * randomRotations[k % randomRotations.Length]));
                    break;

				case '-':
					transform.Rotate(Vector3.forward * angle * (1 + variance / 100 * randomRotations[k % randomRotations.Length]));
                    break;

				case '*':
                    transform.Rotate(Vector3.up * 120f * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;

				case '/':
					transform.Rotate(Vector3.down * 120f * (1f + variance / 100f * randomRotations[k % randomRotations.Length]));
                    break;


                case '[':
                    transformStack.Push(new SavedTransform(transform.position, transform.rotation));
                    break;


				case ']':
                    SavedTransform savedTransform = transformStack.Pop();

                    transform.position = savedTransform.Position;
                    transform.rotation = savedTransform.Rotation;
                    break;
            }

			//Calculating the bounds to make sure that our newly generated tree can be displayed
            boundsMinMaxX.x = Mathf.Min(transform.position.x, boundsMinMaxX.x);
			boundsMinMaxX.y = Mathf.Max(transform.position.x, boundsMinMaxX.y);
			boundsMinMaxY.x = Mathf.Min(transform.position.y, boundsMinMaxY.x);
			boundsMinMaxY.y = Mathf.Max(transform.position.y, boundsMinMaxY.y);
        }

		//Applying the calculated bounds minimum and maximum in comparison to the rect to our camera for scaling the viewport to our tree.
		float aspect = (float)Screen.width / Screen.height;
        Vector3 treeCentre = new Vector3(boundsMinMaxX.x + boundsMinMaxX.y, boundsMinMaxY.x + boundsMinMaxY.y) * .5f;
        float treeHeight = boundsMinMaxY.y - boundsMinMaxY.x;
		float treeWidth = boundsMinMaxX.y - boundsMinMaxX.x;
        float treeSize = Mathf.Max(treeHeight, treeWidth*aspect);


    }

	public void SwitchRules(Rules rule)
	{
        currentlyAppliedRules = rule;
        TranslateRulesToDictionary();
        Generate();
    }

	public void TranslateRulesToDictionary()
	{
		if(personalizedRules.Contains(currentlyAppliedRules))
		{
            rules.Clear();
            for (int i = 0; i < currentlyAppliedRules.rules.Count; i++)
			{
                rules.Add(currentlyAppliedRules.rules[i].Name, currentlyAppliedRules.rules[i].addition);
            }
        }
	}

	void OnDrawGizmos() {
        Gizmos.DrawWireCube(new Vector3(boundsMinMaxX.x + boundsMinMaxX.y, boundsMinMaxY.x + boundsMinMaxY.y) * .5f, new Vector3(boundsMinMaxX.y - boundsMinMaxX.x, boundsMinMaxY.y - boundsMinMaxY.x));
    }
}
