//TODO: Find out how to add extra dependencies
const functions = require('firebase-functions');
const admin = require('firebase-admin');

admin.initializeApp();

exports.webhook = functions.https.onRequest((request, response) => {
    //All logic here
    receivedText = request.body.queryResult.queryText;
    triggeredIntent = request.body.queryResult.intent.displayName;
    ClientName = "test";
    namevar = "test";
    const db = admin.firestore()
    const userRef = db.collection('Users').doc('test-user');

    if (triggeredIntent === "NameToBirthdate")
    {
       userRef.update({name:receivedText});

       //response.send({fulfillmentText: userRef.get({name}+ ", what is your birthdate?")});
    }
    if (triggeredIntent === "Birthdate to Problem")
    {
        userRef.update({dob:receivedText});
        //response.send({fulfillmentText: namevar + " has been saved"})
        
    }
    if (triggeredIntent === "ProblemToLocation")
    {
        userRef.update({mainProblem:receivedText});     
    }
    if (triggeredIntent === "LocationToTime")
    {
        var bodypartRanking = { 
            "heart" : "10" ,
            "head" : "8",
            "leg" : "5",
            "arm" : "4",
            };
        
        for(var key in bodypartRanking)
        {
          if (receivedText.toLowerCase().includes(key)){
            userRef.update({priority:bodypartRanking[key].toString()});
            var HitTrue = "True";
            break;
          }
          
        }
        if (HitTrue !== "True")
        {
            userRef.update({priority: "UNKNOWN"});
        }
       
        
        userRef.update({locationProblem:receivedText});
              
    }
    if (triggeredIntent === "TimeToMoreProblems")
    {
        userRef.update({Timespan:receivedText});       
    }
    if (triggeredIntent === "MakeAppointment")
    {
        userRef.update({appointment:"True"});       
    }
    if (triggeredIntent === "NoAppointment")
    {
        userRef.update({appointment:"False"});       
    }
    // testvar = "hello";

    //response documentation: https://dialogflow.com/docs/fulfillment
    // response.send(
    //     {followupEventInput:{name: "Event1"}}
    // );
});
