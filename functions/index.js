//TODO: Find out how to add extra dependencies
const functions = require('firebase-functions');


exports.webhook = functions.https.onRequest((request, response) => {
    //All logic here
    receivedText = request.body.queryResult.queryText;
    triggeredIntent = request.body.queryResult.intent.displayName;
   
    if (triggeredIntent === "Test")
    {
        ifText = "the triggered intent is Test";
    }
    else
    {
        ifText = "did not hit";
    }
    
    // testvar = "hello";

    //response documentation: https://dialogflow.com/docs/fulfillment
    response.send(
        {fulfillmentText:"user said: " + receivedText + " and " + ifText}
    );
});
