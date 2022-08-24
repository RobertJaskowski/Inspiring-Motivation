using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using TMPro;
using ScriptableObjectArchitecture;
using NaughtyAttributes;
using Leguar.TotalJSON;
using System.Threading.Tasks;
using System.Linq;
using DG.Tweening;

public class SendToFirebase : MonoBehaviour
{

    #region Instance

    public static SendToFirebase instance;

    private void Instantiate()
    {
        if (instance != null)
        {
            Debug.Log("SendToFirebase already exists");
        }
        else
        {
            instance = this;

        }

    }

    #endregion

    public TMP_InputField provideAnswerInputField;
    public TMP_InputField askQuestionInputField;

    [Space(20)]
    public StringGameEvent headerTextEvent;
    public StringGameEvent sendingTextErrorEvent;

    public GameEvent startedLoadingEvent;
    public GameEvent finishedLoadingEvent;

    [Space(20)]

    public GameObject questionResultContentList;
    public GameObject answerResultContentList;
    public GameObject resultContentList;
    public GameObject resultItemPrefab;

    private DatabaseReference dataRef;


    public string startingQuestionId = "-M-zGd0FqfNzPpWbMxRZ";

    public string currentHeaderId;

    List<string> answersIds = new List<string>();

    string que;

    private void Awake()
    {
        Instantiate();
    }

    void Start()
    {
       // InitEditorTests();
        SetDatabaseReference();


        GetStart();
        //SetStartingQuestion();
        //GetStartingResults();
    }

    public void RefreshResults()
    {
        ClearAllChildrenResults();
        GetResults();
    }

    public void GoToHome()
    {
        ClearAllChildrenResults();

        GetStart();
    }


    public void OnAnswerChosen(string headerID, string text)
    {
        startedLoadingEvent.Raise();
        ClearAllChildrenResults();
        headerTextEvent.Raise(text);

        currentHeaderId = headerID;

        GetResults();
    }

    public void OnQuestionChosen(string headerID)
    {
        startedLoadingEvent.Raise();


        currentHeaderId = headerID;

        GetResults();

    }

    public void ClearAllChildrenResults()
    {

        answersIds = new List<string>();

        int childs = answerResultContentList.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(answerResultContentList.transform.GetChild(i).gameObject);
        }

        int childss = questionResultContentList.transform.childCount;
        for (int i = childss - 1; i >= 0; i--)
        {
            Destroy(questionResultContentList.transform.GetChild(i).gameObject);
        }

    }


    public void SetStartingQuestion()
    {
        StartingQuestion question = new StartingQuestion("What do you struggle with?");



        string json = JsonUtility.ToJson(question);

        //string key = dataRef.Child("StartingQuestion").Push().Key;
        dataRef.Child("StartingQuestion").SetRawJsonValueAsync(json);

        //startingQuestionId = key;
        // currentQuestionId = key;
    }


    public Task GetStartingQuestionId()
    {
        return dataRef.Child("StartingQuestion").GetValueAsync()
            .ContinueWith(task =>
            {

                if (task.IsCompleted)
                {

                    StartingQuestion startQuestionInfo = JsonUtility.FromJson<StartingQuestion>(task.Result.GetRawJsonValue());
                    startingQuestionId = startQuestionInfo.questionId;
                    currentHeaderId = startQuestionInfo.questionId;




                }

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("task faulted");
                }

            });
    }

    public Task GetQuestion()
    {
        return dataRef.Child("Nodes").Child("Question").Child(currentHeaderId)
                    .GetValueAsync()
                    .ContinueWith(t =>
                    {
                        // Debug.Log("continue with getting the question ");

                        if (t.IsCompleted)
                        {

                            DataSnapshot snap = t.Result;

                            Question question = JsonUtility.FromJson<Question>(snap.GetRawJsonValue());

                            Debug.Log("question is " + question.question);


                            que = question.question;

                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                headerTextEvent.Raise(que);

                            });


                        }

                        if (t.IsFaulted || t.IsCanceled)
                        {
                            Debug.Log("task faulted at getting question ");
                        }
                    });
    }

    public void GetStart()
    {

        startedLoadingEvent.Raise();

        GetStartingQuestionId()
            .ContinueWith(task =>
            {
                GetQuestion();
            })
            .ContinueWith(task =>
            {

                GetResults();

                
            });
    }

    public void GetStartingResults()
    {
        dataRef.Child("StartingQuestion").GetValueAsync()
            .ContinueWith(task =>
            {

                if (task.IsCompleted)
                {

                    StartingQuestion startQuestionInfo = JsonUtility.FromJson<StartingQuestion>(task.Result.GetRawJsonValue());
                    startingQuestionId = startQuestionInfo.questionId;
                    currentHeaderId = startQuestionInfo.questionId;




                }

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("task faulted");
                }

            })

            .ContinueWith(task =>
            {
                dataRef.Child("Nodes").Child("Question").Child(currentHeaderId)
                    .GetValueAsync()
                    .ContinueWith(t =>
                    {
                        // Debug.Log("continue with getting the question ");

                        if (t.IsCompleted)
                        {

                            DataSnapshot snap = t.Result;

                            Question question = JsonUtility.FromJson<Question>(snap.GetRawJsonValue());

                            Debug.Log("question is " + question.question);


                            que = question.question;

                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                headerTextEvent.Raise(que);

                            });


                        }

                        if (t.IsFaulted || t.IsCanceled)
                        {
                            Debug.Log("task faulted at getting question ");
                        }
                    });
            });

    }

    public void GetResults()
    {


        dataRef.Child("Links").Child(currentHeaderId)
            .GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    //Debug.Log(snapshot.GetRawJsonValue());



                    string json = snapshot.GetRawJsonValue();
                    JSONObject jsonObj = new JSONObject(json);



                    for (int i = 0; i < jsonObj.list.Count; i++)
                    {

                        JSONObject k = (JSONObject)jsonObj.list[i];

                        answersIds.Add(k.str);

                    }





                }

            }).ContinueWith(tt =>
            {
                dataRef.Child("Nodes").Child("Answer").GetValueAsync()
                .ContinueWith(AnswerTask =>
                {
                    DataSnapshot snap = AnswerTask.Result;


                    string json = snap.GetRawJsonValue();
                    JSONObject jsonObject = new JSONObject(json);

                    for (int i = 0; i < jsonObject.list.Count; i++)
                    {
                        JSONObject k = (JSONObject)jsonObject.list[i];





                        Dictionary<string, string> answers = k.ToDictionary();

                        //Debug.Log(questions.ToString());


                        foreach (string answerId in answersIds)
                        {

                            if (answerId == jsonObject.keys[i])
                            {

                                //UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.Log("answerId " + answerId + " jsonobjectkey " + jsonObject.keys[i].ToString() + " questionselementat " + questions.ElementAt(i).Value));
                                UnityMainThreadDispatcher.Instance().Enqueue(() => AddAnswerResultToList(answers["answer"], answerId));

                            }
                        }


                    }



                });



            }).ContinueWith(t =>
            {
                dataRef.Child("Nodes").Child("Question").GetValueAsync()
                .ContinueWith(QuestionTask =>
                {
                    DataSnapshot snap = QuestionTask.Result;


                    string json = snap.GetRawJsonValue();
                    JSONObject jsonObject = new JSONObject(json);

                    for (int i = 0; i < jsonObject.list.Count; i++)
                    {
                        JSONObject k = (JSONObject)jsonObject.list[i];





                        Dictionary<string, string> questions = k.ToDictionary();

                        //Debug.Log(questions.ToString());


                        foreach (string answerId in answersIds)
                        {
                            //Debug.Log("keyyy " + jsonObject.keys[i]);
                            if (answerId == jsonObject.keys[i])
                            {

                                //UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.Log("answerId " + answerId + " jsonobjectkey " + jsonObject.keys[i].ToString() + " questionselementat " + questions.ElementAt(i).Value));
                                UnityMainThreadDispatcher.Instance().Enqueue(() => AddQuestionResultToList(questions["question"], answerId));
                                //AddResultToList(questions["question"]);
                            }
                        }


                        //UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        //{



                        //    for (int b = 0; b < jsonObject.keys.Count; b++)
                        //    {
                        //        Debug.Log("key is " + jsonObject.keys[b]);
                        //    }


                        //    Debug.Log(k + "  asdddddddddddddddddddddddddddddddddddddd");



                        //    foreach (string item in questions.Values)
                        //    {

                        //        foreach (string answerId in answersIds)
                        //        {
                        //            Debug.Log(item + answerId);

                        //            if (answerId == item)
                        //            {
                        //                AddResultToList(item);
                        //            }
                        //        }
                        //    }
                        //});

                    }

                });
            }).ContinueWith(tet =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => finishedLoadingEvent.Raise());
            });
    }

    public bool IsValidTextProvided(string text)
    {
        if (text == null || text.Trim() == "")
        {
            return false;
        }

        return true;
    }

    public void SendQuestion()
    {
        string questionText = askQuestionInputField.text;
        askQuestionInputField.text = "";

        if (!IsValidTextProvided(questionText))
        {

            sendingTextErrorEvent.Raise();
            return;
        }


        Question question = new Question(questionText);

        string json = JsonUtility.ToJson(question);

        string key = dataRef.Child("Nodes").Child("Question").Push().Key;
        dataRef.Child("Nodes").Child("Question").Child(key).SetRawJsonValueAsync(json);
        headerTextEvent.Raise(questionText);



        SendReference(currentHeaderId, key);



    }

    public void SendReference(string originalID, string referenceToAdd)
    {

        QuestionReference refe = new QuestionReference(referenceToAdd);

        string json = JsonUtility.ToJson(refe);

        dataRef
            .Child("Links")
            .Child(originalID)
            .Push()
            //.SetRawJsonValueAsync(json);
            .SetValueAsync(referenceToAdd);


    }

    public void SendAnswer()
    {
        string answerText = provideAnswerInputField.text;
        provideAnswerInputField.text = "";
        Debug.Log("sending answer");
        if (!IsValidTextProvided(answerText))
        {

            sendingTextErrorEvent.Raise();
            return;
        }


        Answer answer = new Answer(answerText);



        string json = JsonUtility.ToJson(answer);

        string key = dataRef.Child("Nodes").Child("Answer").Push().Key;
        dataRef.Child("Nodes").Child("Answer").Child(key).SetRawJsonValueAsync(json);
        headerTextEvent.Raise(answerText);


        SendReference(currentHeaderId, key);

    }

    private void SetDatabaseReference()
    {
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private static void InitEditorTests()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://inspiring-motivation-d7126.firebaseio.com/");
    }


    public void AddResultToList(string text)
    {
        ResultItem item = Instantiate(resultItemPrefab, resultContentList.transform).GetComponent<ResultItem>();
        item.SetTextOfResult(text);
    }

    public void AddQuestionResultToList(string text, string answerId)
    {
        ResultItem item = Instantiate(resultItemPrefab, questionResultContentList.transform).GetComponent<ResultItem>();
        item.SetTextOfResult(text);
        item.SetId(answerId);

        item.SetColor(ResultItemType.Question);
    }

    public void AddAnswerResultToList(string text, string answerId)
    {
        ResultItem item = Instantiate(resultItemPrefab, answerResultContentList.transform).GetComponent<ResultItem>();
        item.SetTextOfResult(text);
        item.SetId(answerId);
        item.SetColor(ResultItemType.Answer);
    }


}

public class StartingQuestion
{

    public string questionId;

    public StartingQuestion(string id)
    {
        this.questionId = id;
    }
}

public class Question
{

    public string question;

    public Question(string question)
    {
        this.question = question;
    }
}

public class QuestionReference
{
    public string reference;

    public QuestionReference(string questionReference)
    {
        this.reference = questionReference;
    }
}


public class ReferenceHolder
{
    public string originalId;
    public List<string> resultsIdList;

    public ReferenceHolder(string question, List<string> resultsIdList)
    {
        this.originalId = question;
        this.resultsIdList = resultsIdList;
    }
}

public class Answer
{
    public string answer;

    public Answer(string answer)
    {
        this.answer = answer;
    }
}

