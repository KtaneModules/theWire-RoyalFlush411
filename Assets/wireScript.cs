using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using wire;

public class wireScript : MonoBehaviour
{
    //selectables
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable intWire;
    public KMSelectable switch1;
    public KMSelectable switch2;
    public KMSelectable switch3;
    public KMSelectable startButton;

    //textureOptions
    public Renderer[] renderers;
    public Renderer intWireRend;
    public Renderer brokenWireA;
    public Renderer brokenWireB;
    public List<Texture> textureOptions;
    int selectedTexture = 0;
    private Texture selectedWireTexture;
    public String[] colourOptions;

    //timer & number
    public TextMesh timerText;
    decimal startTime = 90.00m;
    public TextMesh numberText;
    int displayedNumber = 0;
    string blank = " ";
    string clockTimeWhole = "";
    string clockTime = "";
    float timeRemaining;

    //startButton
    public Renderer startButtonRend;
    private bool startLock = false;
    private int distance = 0;
    private bool wireLock = false;
    public Renderer startButtonLight;
    public Texture lightOn;
    public Texture lightOff;
    private bool lightOnBool = false;
    private bool blinkLock = false;
    private int buttonDistance = 0;
    private bool startAnimationDone = false;
    private int initiated = 0;
    private bool moduleDone = false;

    //switch1
    private int numberOfTurns1 = 0;
    private bool turnLock1 = false;
    private string switch1Set = "Q";

    //switch2
    private int numberOfTurns2 = 0;
    private bool turnLock2 = false;
    private string switch2Set = "S";

    //switch3
    private int numberOfTurns3 = 0;
    private bool turnLock3 = false;
    private string switch3Set = "T";

    //flowChartVariables
    private bool numbersLogged = false;
    private int serialNumberSum = 0;
    private int serialPlusDisplay = 0;
    public char[] letterEquivalents;
    private char numberToLetter;
    private int batteries = 0;
    private int displayPlusBatteries = 0;
    private int lettersInDial2Colour = 0;
    private int lettersInWireColour = 0;
    private int wirePlusDial2Letters = 0;
    private int ports = 0;

    //vennVariables
    private int varA;
    private int varB;
    private int varC;
    private int varD;
    private int varE;
    private int varF;
    private int varG;
    private int varH;
    private int varI;
    private int varJ;

    //vennBools
    private bool vennDial1 = false;
    private bool vennDial2 = false;
    private bool vennDial3 = false;
    private bool vennWire = false;

    //correctAnswers
    private string dial1Answer = "";
    private string dial2Answer = "";
    private string dial3Answer = "";
    private string wholeTime = "";
    private string stopTime = "";

    //logging
    static int moduleIdCounter = 1;
    int moduleId;

    void Update()
    {
        timeRemaining = Bomb.GetTime();
    }

    void Awake()
    {
        moduleId = moduleIdCounter++;
        intWire.OnInteract += delegate () { OnintWire(); return false; };
        switch1.OnInteract += delegate () { Onswitch1(); return false; };
        switch2.OnInteract += delegate () { Onswitch2(); return false; };
        switch3.OnInteract += delegate () { Onswitch3(); return false; };
        startButton.OnInteract += delegate () { OnstartButton(); return false; };
    }

    void Start()
    {
        vennDial1 = false;
        vennDial2 = false;
        vennDial3 = false;
        vennWire = false;
        blinkLock = false;
        wireLock = false;
        intWire.gameObject.SetActive(false);
        switch1.gameObject.SetActive(false);
        switch2.gameObject.SetActive(false);
        switch3.gameObject.SetActive(false);
        if (moduleDone == false)
        {
            brokenWireA.gameObject.SetActive(false);
            brokenWireB.gameObject.SetActive(false);
        }
        numberText.text = blank;
        timerText.text = blank;
        startAnimationDone = false;
        startLock = false;
        serialNumberSum = Bomb.GetSerialNumberNumbers().Sum();
        batteries = Bomb.GetBatteryCount();
        ports = Bomb.GetPortCount();
        if (numbersLogged == false)
        {
            Debug.LogFormat("[The Wire #{0}] The sum of the serial number digits is {1}.", moduleId, serialNumberSum);
            numbersLogged = true;
        }
    }

    //Logic
    void flowStart() //Start of flow chart
    {
        dial1Answer = "";
        dial2Answer = "";
        dial3Answer = "";
        Debug.LogFormat("[The Wire #{0}] ___START OF FLOW CHART #{1}___", moduleId, initiated);
        serialPlusDisplay = serialNumberSum + displayedNumber;
        Debug.LogFormat("[The Wire #{0}] The sum of the serial number digits and the displayed number is {1}.", moduleId, serialPlusDisplay);
        numberToLetter = letterEquivalents[serialPlusDisplay];
        Debug.LogFormat("[The Wire #{0}] The letter equivalent of {1} is {2}.", moduleId, serialPlusDisplay, numberToLetter);

        if (Bomb.GetSerialNumberLetters().Any(x => x == numberToLetter))
        {
            Debug.LogFormat("[The Wire #{0}] {1} does appear in the serial number. Take the 'yes' path.", moduleId, numberToLetter);
            if (dial1Answer == "")
            {
                dial1Answer = "Q";
                Debug.LogFormat("[The Wire #{0}] The first dial should be set to Q.", moduleId);
                flowTwo();
            }
            else
            {
                Debug.LogFormat("[The Wire #{0}] The first dial should not be changed.", moduleId);
                flowTwo();
            }
        }
        else
        {
            Debug.LogFormat("[The Wire #{0}] {1} does not appear in the serial number. Take the 'no' path.", moduleId, numberToLetter);
            flowThree();
        }
    }

    void flowTwo() //Displayed number + batteries
    {
        displayPlusBatteries = displayedNumber + batteries;
        Debug.LogFormat("[The Wire #{0}] The displayed number plus the number of batteries is {1}.", moduleId, displayPlusBatteries);

        if (renderers[1].material.mainTexture == textureOptions[0] || renderers[1].material.mainTexture == textureOptions[2])
        {
            lettersInDial2Colour = 4;
        }
        else if (renderers[1].material.mainTexture == textureOptions[3] || renderers[1].material.mainTexture == textureOptions[4])
        {
            lettersInDial2Colour = 6;
        }
        else if (renderers[1].material.mainTexture == textureOptions[1])
        {
            lettersInDial2Colour = 5;
        }
        else
        {
            lettersInDial2Colour = 3;
        }

        if (intWireRend.material.mainTexture == textureOptions[0] || intWireRend.material.mainTexture == textureOptions[2])
        {
            lettersInWireColour = 4;
        }
        else if (intWireRend.material.mainTexture == textureOptions[3] || intWireRend.material.mainTexture == textureOptions[4])
        {
            lettersInWireColour = 6;
        }
        else if (intWireRend.material.mainTexture == textureOptions[1])
        {
            lettersInWireColour = 5;
        }
        else
        {
            lettersInWireColour = 3;
        }
        wirePlusDial2Letters = lettersInWireColour + lettersInDial2Colour;
        Debug.LogFormat("[The Wire #{0}] There are {1} combined letters in the names of the dial 2 and wire colours.", moduleId, wirePlusDial2Letters);

        if (displayPlusBatteries > wirePlusDial2Letters)
        {
            Debug.LogFormat("[The Wire #{0}] {1} exceeds {2}. Take the 'yes' path.", moduleId, displayPlusBatteries, wirePlusDial2Letters);
            flowSix();
        }
        else
        {
            Debug.LogFormat("[The Wire #{0}] {1} does not exceed {2}. Take the 'no' path.", moduleId, displayPlusBatteries, wirePlusDial2Letters);
            if (dial2Answer == "")
            {
                dial2Answer = "E";
                Debug.LogFormat("[The Wire #{0}] The second dial should be set to E.", moduleId);
                flowFour();
            }
            else
            {
                Debug.LogFormat("[The Wire #{0}] The second dial should not be changed.", moduleId);
                flowFour();
            }
        }
    }

    void flowThree() //any dial = wire colour
    {
        if (renderers.Any((x) => x.material.mainTexture == intWireRend.material.mainTexture))
        {
            Debug.LogFormat("[The Wire #{0}] At least one of the dial colours matches the wire colour. Take the 'yes' path.", moduleId);
            flowTwo();
        }
        else
        {
            Debug.LogFormat("[The Wire #{0}] None of the dial colours match the wire colour. Take the 'no' path.", moduleId);
            flowFive();
        }
    }
    void flowFour() //number + 1 = prime?
    {
        if (displayedNumber == 1 || displayedNumber == 2 || displayedNumber == 4|| displayedNumber == 6)
        {
            Debug.LogFormat("[The Wire #{0}] {1} + 1 = {2}, which is prime. Take the 'yes' path.", moduleId, displayedNumber, displayedNumber + 1);
            flowEight();
        }
        else
        {
            Debug.LogFormat("[The Wire #{0}] {1} + 1 = {2}, which is not prime. Take the 'no' path.", moduleId, displayedNumber, displayedNumber + 1);
            if (dial3Answer == "")
            {
                dial3Answer = "T";
                Debug.LogFormat("[The Wire #{0}] The third dial should be set to T.", moduleId);
                flowSeven();
            }
            else
            {
                Debug.LogFormat("[The Wire #{0}] The third dial should not be changed.", moduleId);
                flowSeven();
            }
        }
    }

    void flowFive() //ports > number?
    {
        if (ports > displayedNumber)
        {
            Debug.LogFormat("[The Wire #{0}] The number of ports ({1}) is greater than the displayed number ({2}). Take the 'yes' path.", moduleId, ports, displayedNumber);
            flowFour();
        }

        else
        {
            Debug.LogFormat("[The Wire #{0}] The number of ports ({1}) is not greater than the displayed number ({2}). Take the 'no' path.", moduleId, ports, displayedNumber);
            flowSeven();
        }
    }

    void flowSix() //green & purple present?
    {
        if ((renderers.Any((x) => x.material.mainTexture == textureOptions[1]) || intWireRend.material.mainTexture == textureOptions[1]) && (renderers.Any((x) => x.material.mainTexture == textureOptions[4]) || intWireRend.material.mainTexture == textureOptions[4]))
        {
            Debug.LogFormat("[The Wire #{0}] Purple & green are both present. Take the 'yes' path.", moduleId);
            if (dial3Answer == "")
            {
                dial3Answer = "Y";
                Debug.LogFormat("[The Wire #{0}] The third dial should be set to Y.", moduleId);
                flowSeven();
            }
            else
            {
                Debug.LogFormat("[The Wire #{0}] The third dial should not be changed.", moduleId);
                flowSeven();
            }
        }
        else
        {
            Debug.LogFormat("[The Wire #{0}] Purple & green are not both present. Take the 'no' path.", moduleId);
            if (dial2Answer == "")
            {
                dial2Answer = "M";
                Debug.LogFormat("[The Wire #{0}] The second dial should be set to M.", moduleId);
                flowFive();
            }
            else
            {
                Debug.LogFormat("[The Wire #{0}] The second dial should not be changed.", moduleId);
                flowFive();
            }
        }
    }

    void flowSeven() //7 or 3 + red wire?
    {
        if (displayedNumber == 7 || displayedNumber == 3 || renderers.Any((x) => x.material.mainTexture == textureOptions[5]) || intWireRend.material.mainTexture == textureOptions[5])
        {
            Debug.LogFormat("[The Wire #{0}] The displayed number is 7 or 3 or the colour red is present. Take the 'yes' path.", moduleId);
            finalFlowD();
        }
        else
        {
            Debug.LogFormat("[The Wire #{0}] The displayed number is not 7 or 3 and the colour red is not present. Take the 'no' path.", moduleId);
            finalFlowC();
        }
    }

    void flowEight() //dial colours match?
    {
        if (renderers.Select((x) => x.material.mainTexture).Distinct().Count() == 3)
        {
            Debug.LogFormat("[The Wire #{0}] All of the dials are different colours. Take the 'no' path.", moduleId);
            finalFlowB();
        }
        else
        {
            Debug.LogFormat("[The Wire #{0}] Two or more of the dials match in colour. Take the 'yes' path.", moduleId);
            finalFlowA();
        }
    }

    void finalFlowA()
    {
        Debug.LogFormat("[The Wire #{0}] ___END OF FLOW CHART #{1}___", moduleId, initiated);
        if (dial1Answer == "")
        {
            dial1Answer = "Q";
        }
        if (dial2Answer == "")
        {
            dial2Answer = "E";
        }
        if (dial3Answer == "")
        {
            dial3Answer = "Y";
        }
        Debug.LogFormat("[The Wire #{0}] Dial 1 should be set to {1}.", moduleId, dial1Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 2 should be set to {1}.", moduleId, dial2Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 3 should be set to {1}.", moduleId, dial3Answer);
    }

    void finalFlowB()
    {
        Debug.LogFormat("[The Wire #{0}] ___END OF FLOW CHART #{1}___", moduleId, initiated);
        if (dial1Answer == "")
        {
            dial1Answer = "I";
        }
        if (dial2Answer == "")
        {
            dial2Answer = "M";
        }
        if (dial3Answer == "")
        {
            dial3Answer = "T";
        }
        Debug.LogFormat("[The Wire #{0}] Dial 1 should be set to {1}.", moduleId, dial1Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 2 should be set to {1}.", moduleId, dial2Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 3 should be set to {1}.", moduleId, dial3Answer);
    }

    void finalFlowC()
    {
        Debug.LogFormat("[The Wire #{0}] ___END OF FLOW CHART #{1}___", moduleId, initiated);
        if (dial1Answer == "")
        {
            dial1Answer = "U";
        }
        if (dial2Answer == "")
        {
            dial2Answer = "S";
        }
        if (dial3Answer == "")
        {
            dial3Answer = "B";
        }
        Debug.LogFormat("[The Wire #{0}] Dial 1 should be set to {1}.", moduleId, dial1Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 2 should be set to {1}.", moduleId, dial2Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 3 should be set to {1}.", moduleId, dial3Answer);
    }

    void finalFlowD()
    {
        Debug.LogFormat("[The Wire #{0}] ___END OF FLOW CHART #{1}___", moduleId, initiated);
        if (dial1Answer == "")
        {
            dial1Answer = "Z";
        }
        if (dial2Answer == "")
        {
            dial2Answer = "A";
        }
        if (dial3Answer == "")
        {
            dial3Answer = "O";
        }
        Debug.LogFormat("[The Wire #{0}] Dial 1 should be set to {1}.", moduleId, dial1Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 2 should be set to {1}.", moduleId, dial2Answer);
        Debug.LogFormat("[The Wire #{0}] Dial 3 should be set to {1}.", moduleId, dial3Answer);
    }

    private void vennVariableCalc()
    {
        varA = displayedNumber;
        varB = initiated;
        varC = (Bomb.GetOffIndicators().Count() + Bomb.GetOnIndicators().Count()) * 2;
        varD = Bomb.GetPortPlateCount() * 4;
        varE = displayedNumber % 3;
        varF = Bomb.GetOffIndicators().Count();
        varG = Bomb.GetPortCount(Port.Parallel) + Bomb.GetPortCount(Port.Serial) + Bomb.GetPortCount(Port.RJ45) + Bomb.GetPortCount(Port.DVI);
        varH = Bomb.GetModuleNames().Count();
        varI = displayedNumber * 6;
        varJ = Bomb.GetOnIndicators().Count();

        if (renderers[0].material.mainTexture == textureOptions[0] || renderers[0].material.mainTexture == textureOptions[1] || renderers[0].material.mainTexture == textureOptions[5])
        {
            vennDial1 = true;
        }
        if (renderers[1].material.mainTexture == textureOptions[0] || renderers[1].material.mainTexture == textureOptions[2] || renderers[1].material.mainTexture == textureOptions[3])
        {
            vennDial2 = true;
        }
        if (renderers[2].material.mainTexture == textureOptions[3] || renderers[2].material.mainTexture == textureOptions[4] || renderers[2].material.mainTexture == textureOptions[5])
        {
            vennDial3 = true;
        }
        if (intWireRend.material.mainTexture == textureOptions[1] || intWireRend.material.mainTexture == textureOptions[2] || intWireRend.material.mainTexture == textureOptions[4])
        {
            vennWire = true;
        }
    }

    private void vennCalculator()
    {
        if (vennDial1 && vennDial2 && vennDial3 && vennWire)
        {
            wholeTime = (3 * varH + varG).ToString();
            stopTime = wholeTime[0].ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is X.", moduleId);
            Debug.LogFormat("[The Wire #{0}] h={1}; g={2}", moduleId, varH, varG);
        }
        else if (vennDial1 && vennDial2 && vennDial3)
        {
            wholeTime = (varG + varA).ToString();
            stopTime = wholeTime[wholeTime.Length - 1].ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is I.", moduleId);
            Debug.LogFormat("[The Wire #{0}] g={1}; a={2}", moduleId, varG, varA);
        }
        else if (vennDial1 && vennDial2 && vennWire)
        {
            stopTime = (((varD % 7) + (varI % 4)) % 10).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is XIII.", moduleId);
            Debug.LogFormat("[The Wire #{0}] d={1}; i={2}", moduleId, varD, varI);
        }
        else if (vennDial1 && vennDial3 && vennWire)
        {
            wholeTime = varD.ToString();
            stopTime = wholeTime[wholeTime.Length - 1].ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is VIII.", moduleId);
            Debug.LogFormat("[The Wire #{0}] d={1}", moduleId, varD);

        }
        else if (vennDial2 && vennDial3 && vennWire)
        {
            stopTime = ((varJ + varE + varF) % 10).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is II.", moduleId);
            Debug.LogFormat("[The Wire #{0}] j={1}; e={2}; f={3}", moduleId, varJ, varE, varF);
        }
        else if (vennDial1 && vennDial2)
        {
            stopTime = ((varB + varC) % 6).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is IV.", moduleId);
            Debug.LogFormat("[The Wire #{0}] b={1}; c={2}", moduleId, varB, varC);
        }
        else if (vennDial1 && vennDial3)
        {
            stopTime = (((varG * varB) + varB) % (varE + 4)).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is VII.", moduleId);
            Debug.LogFormat("[The Wire #{0}] g={1}; b={2}; e={3}", moduleId, varG, varB, varE);
        }
        else if (vennDial1 && vennWire)
        {
            stopTime = ((varJ * (varF + varH)) % 9).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is XV.", moduleId);
            Debug.LogFormat("[The Wire #{0}] j={1}; f={2}; h={3}", moduleId, varJ, varF, varH);
        }
        else if (vennDial2 && vennDial3)
        {
            stopTime = ((4 * varJ) % 5).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is XII.", moduleId);
            Debug.LogFormat("[The Wire #{0}] j={1}", moduleId, varJ);
        }
        else if (vennDial2 && vennWire)
        {
            wholeTime = ((varF * varI) + varH).ToString();
            stopTime = wholeTime[0].ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is VI.", moduleId);
            Debug.LogFormat("[The Wire #{0}] f={1}; i={2}; h={3}", moduleId, varF, varI, varH);
        }
        else if (vennDial3 && vennWire)
        {
            stopTime = ((varI + varD - varE) % 10).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is XI.", moduleId);
            Debug.LogFormat("[The Wire #{0}] i={1}; d={2}; e={3}", moduleId, varI, varD, varE);

        }
        else if (vennDial1)
        {
            stopTime = ((varJ * varA + varC) % 9).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is V.", moduleId);
            Debug.LogFormat("[The Wire #{0}] j={1}; a={2}; c={3}", moduleId, varJ, varA, varC);
        }
        else if (vennDial2)
        {
            stopTime = ((varA * varC * varF) % 8).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is IX.", moduleId);
            Debug.LogFormat("[The Wire #{0}] a={1}; c={2}; f={3}", moduleId, varA, varC, varF);
        }
        else if (vennDial3)
        {
            wholeTime = (varC * varG).ToString();
            stopTime = wholeTime[wholeTime.Length - 1].ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is XIV.", moduleId);
            Debug.LogFormat("[The Wire #{0}] c={1}; g={2}", moduleId, varC, varG);
        }
        else if (vennWire)
        {
            stopTime = (((varI + varD + varH) % 7) + 2).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is III.", moduleId);
            Debug.LogFormat("[The Wire #{0}] i={1}; d={2}; h={3}", moduleId, varI, varD, varH);
        }
        else
        {
            stopTime = (((varE + varA) * varB) % 8).ToString();
            Debug.LogFormat("[The Wire #{0}] The Venn reference is XVI.", moduleId);
            Debug.LogFormat("[The Wire #{0}] e={1}; a={2}; b={3}", moduleId, varE, varA, varB);
        }
        Debug.LogFormat("[The Wire #{0}] You must cut the wire when the last digit of the second timer is a {1}.", moduleId, stopTime);
    }

    ///Interactables
    public void Onswitch1()
    {
        if (turnLock1)
        {

        }
        else
        {
            Audio.PlaySoundAtTransform("dial", transform);
            turnLock1 = true;
            StartCoroutine(switch1Turn());
        }
    }

    private IEnumerator switch1Turn()
    {
        while(numberOfTurns1 != 20)
        {
            yield return new WaitForSeconds(0.001f);
            renderers[0].transform.Rotate(Vector3.up, 4.5f);
            numberOfTurns1 += 1;
        }
        if (switch1Set == "Q")
        {
            switch1Set = "I";
        }
        else if (switch1Set == "I")
        {
            switch1Set = "Z";
        }
        else if (switch1Set == "Z")
        {
            switch1Set = "U";
        }
        else if (switch1Set == "U")
        {
            switch1Set = "Q";
        }
        numberOfTurns1 = 0;
        turnLock1 = false;
    }

    public void Onswitch2()
    {
        if (turnLock2)
        {

        }
        else
        {
            Audio.PlaySoundAtTransform("dial", transform);
            turnLock2 = true;
            StartCoroutine(switch2Turn());
        }
    }

    private IEnumerator switch2Turn()
    {
        while(numberOfTurns2 != 20)
        {
            yield return new WaitForSeconds(0.001f);
            renderers[1].transform.Rotate(Vector3.up, 4.5f);
            numberOfTurns2 += 1;
        }
        if (switch2Set == "S")
        {
            switch2Set = "M";
        }
        else if (switch2Set == "M")
        {
            switch2Set = "E";
        }
        else if (switch2Set == "E")
        {
            switch2Set = "A";
        }
        else if (switch2Set == "A")
        {
            switch2Set = "S";
        }
        numberOfTurns2 = 0;
        turnLock2 = false;
    }

    public void Onswitch3()
    {
        if (turnLock3)
        {

        }
        else
        {
            Audio.PlaySoundAtTransform("dial", transform);
            turnLock3 = true;
            StartCoroutine(switch3Turn());
        }
    }

    private IEnumerator switch3Turn()
    {
        while(numberOfTurns3 != 20)
        {
            yield return new WaitForSeconds(0.001f);
            renderers[2].transform.Rotate(Vector3.up, 4.5f);
            numberOfTurns3 += 1;
        }
        if (switch3Set == "T")
        {
            switch3Set = "B";
        }
        else if (switch3Set == "B")
        {
            switch3Set = "Y";
        }
        else if (switch3Set == "Y")
        {
            switch3Set = "O";
        }
        else if (switch3Set == "O")
        {
            switch3Set = "T";
        }
        numberOfTurns3 = 0;
        turnLock3 = false;
    }

    public void OnstartButton()
    {
        //Logic about picking colours for wire & switches
        if (startLock || wireLock || blinkLock || moduleDone)
        {

        }
        else
        {
            initiated ++;
            Debug.LogFormat("[The Wire #{0}] ___SYSTEM INITIATED: ITERATION #{1}___", moduleId, initiated);
            startLock = true;
            blinkLock = true;
            wireLock= true;
            intWire.gameObject.SetActive(true);
            switch1.gameObject.SetActive(true);
            switch2.gameObject.SetActive(true);
            switch3.gameObject.SetActive(true);
            int wireTextureIndex = UnityEngine.Random.Range(0,6);
            selectedWireTexture = textureOptions[wireTextureIndex];
            string chosenWireColour = colourOptions[wireTextureIndex];
            Debug.LogFormat("[The Wire #{0}] The wire colour is {1}.", moduleId, chosenWireColour);
            intWireRend.material.mainTexture = selectedWireTexture;
            brokenWireA.material.mainTexture = selectedWireTexture;
            brokenWireB.material.mainTexture = selectedWireTexture;

            foreach (Renderer rend in renderers)
            {
                selectedTexture = UnityEngine.Random.Range(0,6);
                rend.material.mainTexture = textureOptions[selectedTexture];
            }

            Debug.LogFormat("[The Wire #{0}] The dial 1 colour is {1}", moduleId, renderers[0].material.mainTexture.name.Replace("Mat", "."));
            Debug.LogFormat("[The Wire #{0}] The dial 2 colour is {1}", moduleId, renderers[1].material.mainTexture.name.Replace("Mat", "."));
            Debug.LogFormat("[The Wire #{0}] The dial 3 colour is {1}", moduleId, renderers[2].material.mainTexture.name.Replace("Mat", "."));

            displayedNumber = UnityEngine.Random.Range(0,10);
            Debug.LogFormat("[The Wire #{0}] The displayed number is {1}.", moduleId, displayedNumber);
            numberText.text = displayedNumber.ToString();
            flowStart();
            vennVariableCalc();
            vennCalculator();
            StartCoroutine(startAnimation());
            StartCoroutine(lightBlink());
            StartCoroutine(dialAnimation());
            StartCoroutine(timer());
        }
    }

    private IEnumerator lightBlink()
    {
        while (blinkLock)
        {
            yield return new WaitForSeconds (1f);
            if (lightOnBool)
            {
                startButtonLight.material.mainTexture = lightOff;
                lightOnBool = false;
            }
            else if (lightOnBool == false)
            {
                startButtonLight.material.mainTexture = lightOn;
                Audio.PlaySoundAtTransform("lightSwitch", transform);
                lightOnBool = true;
            }
        }
    startButtonLight.material.mainTexture = lightOff;
    }

    private IEnumerator startAnimation()
    {
        while (buttonDistance < 20)
        {
            yield return new WaitForSeconds(0.005f);
            startButtonRend.transform.localPosition = startButtonRend.transform.localPosition + Vector3.up * -4f;
            buttonDistance ++;
        }
        buttonDistance = 0;
    }

    private IEnumerator reverseStartAnimation()
    {
        while (buttonDistance < 20)
        {
            yield return new WaitForSeconds(0.005f);
            startButtonRend.transform.localPosition = startButtonRend.transform.localPosition + Vector3.up * 4f;
            buttonDistance ++;
        }
        buttonDistance = 0;
        startAnimationDone = true;
    }

    private IEnumerator dialAnimation()
    {
        Audio.PlaySoundAtTransform("hiss", transform);
        while (distance < 100)
        {
            yield return new WaitForSeconds(0.04f);
            intWireRend.transform.localPosition = intWireRend.transform.localPosition + Vector3.up * 0.000125f;
            brokenWireA.transform.localPosition = brokenWireA.transform.localPosition + Vector3.up * 0.000125f;
            brokenWireB.transform.localPosition = brokenWireB.transform.localPosition + Vector3.up * 0.000125f;
            renderers[0].transform.localPosition = renderers[0].transform.localPosition + Vector3.up * 0.000125f;
            renderers[1].transform.localPosition = renderers[1].transform.localPosition + Vector3.up * 0.000125f;
            renderers[2].transform.localPosition = renderers[2].transform.localPosition + Vector3.up * 0.000125f;
            distance ++;
        }
    distance = 0;
    wireLock = false;
    }

    private IEnumerator timer()
    {
        while (startTime > 0)
        {
            yield return new WaitForSeconds(0.017f);
            startTime = startTime - 0.02m;
            timerText.text = startTime.ToString();
            if (startTime == 15.00m || startTime == 10.00m || startTime == 5.00m)
            {
                Audio.PlaySoundAtTransform("warning", transform);
            }
        }
        wireLock = true;
        Audio.PlaySoundAtTransform("hiss", transform);
        numberText.text = blank;
        timerText.text = blank;
        while (distance < 100)
        {
            yield return new WaitForSeconds(0.04f);
            if (moduleDone == false)
            {
                intWireRend.transform.localPosition = intWireRend.transform.localPosition + Vector3.up * -0.000125f;
                brokenWireA.transform.localPosition = brokenWireA.transform.localPosition + Vector3.up * -0.000125f;
                brokenWireB.transform.localPosition = brokenWireB.transform.localPosition + Vector3.up * -0.000125f;
            }
            renderers[0].transform.localPosition = renderers[0].transform.localPosition + Vector3.up * -0.000125f;
            renderers[1].transform.localPosition = renderers[1].transform.localPosition + Vector3.up * -0.000125f;
            renderers[2].transform.localPosition = renderers[2].transform.localPosition + Vector3.up * -0.000125f;
            distance ++;
        }
        StartCoroutine(reverseStartAnimation());
        distance = 0;
        startTime = 90.00m;
        yield return new WaitUntil(() => startAnimationDone);
        Start();
    }

    public void OnintWire()
    {
        if (wireLock)
        {

        }
        else
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSnip, transform);
            startTime = 1.00m;

            clockTimeWhole = Bomb.GetFormattedTime();
            if (timeRemaining > 60f)
            {
                clockTime = clockTimeWhole[clockTimeWhole.Length - 1].ToString();
            }
            else
            {
                clockTime = clockTimeWhole[clockTimeWhole.Length - 4].ToString();
            }

            if (switch1Set == dial1Answer && switch2Set == dial2Answer && switch3Set == dial3Answer)
            {
                if (clockTime == stopTime)
                {
                    wireLock = true;
                    brokenWireA.gameObject.SetActive(true);
                    brokenWireB.gameObject.SetActive(true);
                    intWire.gameObject.SetActive(false);
                    GetComponent<KMBombModule>().HandlePass();
                    moduleDone = true;
                    Debug.LogFormat("[The Wire #{0}] Your dials were set correctly and you cut the wire at the correct time. Module disarmed.", moduleId);
                }
                else
                {
                    brokenWireA.gameObject.SetActive(true);
                    brokenWireB.gameObject.SetActive(true);
                    intWire.gameObject.SetActive(false);
                    GetComponent<KMBombModule>().HandleStrike();
                    Debug.LogFormat("[The Wire #{0}] Strike! Your dials were set correctly but you cut the wire when the last digit of the second timer was {1}. That is incorrect.", moduleId, clockTime);
                }
            }
            else
            {
                brokenWireA.gameObject.SetActive(true);
                brokenWireB.gameObject.SetActive(true);
                intWire.gameObject.SetActive(false);
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[The Wire #{0}] Strike! Your dials were set to {1}, {2} & {3}. That is incorrect.", moduleId, switch1Set, switch2Set, switch3Set);
            }
        }
    }

	#pragma warning disable 414
	private string TwitchHelpMessage = "Start the module using !{0} initalize. Set the dials using !{0} set <dial number> <letter>. Multiple dial and letter pairs can be given. Cut the wire based on the seconds digit using !{0} cut at <number>.";
	#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string inputCommand)
	{
		inputCommand = System.Text.RegularExpressions.Regex.Replace(inputCommand, "^cut (at|on) ", "cut ");
		string[] split = inputCommand.ToLowerInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		if (split.Length == 1 && (split[0] == "initiate" || split[0] == "initalize" || split[0] == "start" || split[0] == "go") && !startLock)
		{
			startButton.OnInteract();
			yield return new WaitForSeconds(0.1f);
		}
		else if (startLock)
		{
			if (split.Length >= 3 && split.Length % 2 == 1 && (split[0] == "set" || split[0] == "dial"))
			{
				// Stores all the actions that the user wants to execute, index 0 is the dial index and index 1 is the target index of that dial.
				List<int[]> actions = new List<int[]>();

				string[] dialLetterSets = new[] { "qizu", "smea", "tbyo" };
				KMSelectable[] dials = new[] { switch1, switch2, switch3 };
				string[] dialLetters = new[] { switch1Set, switch2Set, switch3Set };

				for (int i = 1; i < split.Length; i += 2)
				{
					int dialPosition;
					if (!int.TryParse(split[i], out dialPosition)) yield break;

					int targetIndex = dialLetterSets[dialPosition - 1].IndexOf(split[i + 1]);
					if (targetIndex == -1) yield break;

					actions.Add(new[] { dialPosition - 1, targetIndex });
				}

				foreach (int[] action in actions.GroupBy(x => x[0]).Select(x => x.Last()))
				{
					int dialIndex = action[0];

					KMSelectable selectable = dials[dialIndex];
					int currentIndex = dialLetterSets[dialIndex].IndexOf(dialLetters[dialIndex], StringComparison.InvariantCultureIgnoreCase);
					for (int i = 0; i < (action[1] - currentIndex + 4) % 4; i++)
					{
						selectable.OnInteract();
						yield return new WaitWhile(() => turnLock1 || turnLock2 || turnLock3);
					}
				}
			}
			else if (split.Length == 2 && split[0] == "cut")
			{
				int seconds;
				if (int.TryParse(split[1], out seconds) && seconds >= 0 && seconds <= 9)
				{
					yield return null;
					while (Mathf.FloorToInt(Bomb.GetTime()) % 10 != seconds) yield return "trycancel Wire wasn't cut due to request to cancel.";

					intWire.OnInteract();
				}
			}
		}
	}
}
