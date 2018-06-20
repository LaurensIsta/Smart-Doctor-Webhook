using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Smartdoctor_api_connection
{
  class Program
  {

    public static void Main(string[] args)
    {
      var starttime = DateTime.Now;
      int appointmentlength = 15;
      var attendees = new string["", "", ""];
      MakeAppointment();
    }

    public void MakeAppointment(DateTime start, int minutes, string[] attendees, string summary = "Standard Summary", string description = "Standard Description", string location = "Hogeschool Rotterdam")
    {
      var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
       new ClientSecrets
       {
         ClientId = "///",
         ClientSecret = "///",
       },
       new[] { CalendarService.Scope.Calendar },
       "user",
       CancellationToken.None).Result;
      
      // Create the service.
      var service = new CalendarService(new BaseClientService.Initializer
      {
        HttpClientInitializer = credential,
        ApplicationName = "Calendar API",
      });

      DateTime end = start.Add(new TimeSpan(0, minutes, 0));
      var curTZone = TimeZone.CurrentTimeZone;
      var dateStart = new DateTimeOffset(start, curTZone.GetUtcOffset(start));
      var dateEnd = new DateTimeOffset(end, curTZone.GetUtcOffset(end));
      var startTimeString = dateStart.ToString("o");
      var endTimeString = dateEnd.ToString("o");
      var myEvent = new Event
      {
        Summary = summary,
        Location = location,
        Description = description,
        Start = new EventDateTime
        {
          DateTime = Convert.ToDateTime(startTimeString),
          TimeZone = "GMT+2:00"
        },
        End = new EventDateTime
        {
          DateTime = Convert.ToDateTime(endTimeString),
          TimeZone = "GMT+2:00"
        },
        //Recurrence = new String[] { "RRULE:FREQ=ONCE;BYDAY=MO" },

        Attendees = new List<EventAttendee> { },

      };

      foreach (string attendee in attendees)
      {
        myEvent.Attendees.Append(new EventAttendee { Email = attendee });
      }

      if (IsOverlapping(start, end, service) == false)
      {
        var recurringEvent = service.Events.Insert(myEvent, "primary");
        recurringEvent.SendNotifications = true;
        recurringEvent.Execute();
      }

      else
      {
        Console.WriteLine("there is already an apppointment scheduled at the given date and time");
      }


    }

    public static bool IsOverlapping(DateTime start, DateTime end, CalendarService service)
    {
      Console.WriteLine("Upcoming events:");

      // Define parameters of request.
      EventsResource.ListRequest request = service.Events.List("primary");
      request.TimeMin = DateTime.Now;
      request.ShowDeleted = false;
      request.SingleEvents = true;
      request.MaxResults = 10;
      request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

      // List events.
      Events events = request.Execute();
      Console.WriteLine("Upcoming events:");
      if (events.Items != null && events.Items.Count > 0)
      {
        foreach (var eventItem in events.Items)
        {
          string when = eventItem.Start.DateTime.ToString();

          if (String.IsNullOrEmpty(when))
          {
            when = eventItem.Start.Date;
          }
          // Response.Write(eventItem.Summary + " " + when);
          Console.WriteLine(eventItem.Summary + " " + when);

          if (((eventItem.Start.DateTime >= start) && (eventItem.Start.DateTime < end)) || ((eventItem.End.DateTime > start) && (eventItem.End.DateTime < end)) || ((eventItem.Start.DateTime < start) && (eventItem.End.DateTime >= end)))
          {
            return true;
            //Console.WriteLine("2 appointments are overlapping");
          }

          else if ((eventItem.Start.DateTime == start) || (eventItem.End.DateTime == end))
          {
            return true;
            //Console.WriteLine("2 appointments are overlapping");
          }
        }

        return false;
                
      }

      else
      {
        return false;
      }
    }


    // If modifying these scopes, delete your previously saved credentials
    // at ~/.credentials/calendar-dotnet-quickstart.json
    static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
    static string ApplicationName = "Calendar API";

    /*static void Main(string[] args)
    {      

      Console.WriteLine("Upcoming events:");

      var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                         new ClientSecrets
                         {
                           ClientId = "48713435978-gckh69rqocv8a71gf3atb3mssvalcapm.apps.googleusercontent.com",
                           ClientSecret = "PnDuXrJrBo090pv9mn0ql66Y",
                         },
                         new[] { CalendarService.Scope.Calendar },
                         "user",
                         CancellationToken.None).Result;

      // Create the service.
      var service = new CalendarService(new BaseClientService.Initializer
      {
        HttpClientInitializer = credential,
        ApplicationName = "Calendar API",
      });

      // Define parameters of request.
      EventsResource.ListRequest request = service.Events.List("primary");
      request.TimeMin = DateTime.Now;
      request.ShowDeleted = false;
      request.SingleEvents = true;
      request.MaxResults = 10;
      request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

      bool overlapping = false;

      DateTime start = DateTime.Now;

      // List events.
      Events events = request.Execute();
      Console.WriteLine("Upcoming events:");
      if (events.Items != null && events.Items.Count > 0)
      {

        foreach (var eventItem in events.Items)
        {
          if (((eventItem.Start.DateTime >= start) && (eventItem.Start.DateTime < end)) || ((eventItem.End.DateTime > start) && (eventItem.End.DateTime < end)) || ((eventItem.Start.DateTime < start) && (eventItem.End.DateTime >= end)))
          {
            overlapping = true;
            //Console.WriteLine("2 appointments are overlapping");
          }

          if ((eventItem.Start.DateTime == start) || (eventItem.End.DateTime == end))
          {
            overlapping = true;
            //Console.WriteLine("2 appointments are overlapping");
          }

          string when = eventItem.Start.DateTime.ToString();
          if (String.IsNullOrEmpty(when))
          {
            when = eventItem.Start.Date;
          }
          // Response.Write(eventItem.Summary + " " + when);
          Console.WriteLine(eventItem.Summary + " " + when);
        }
      }

      else
      {
        Console.WriteLine("No upcoming events found.");
      }


      var curTZone = TimeZone.CurrentTimeZone;
      var dateStart = new DateTimeOffset(start, curTZone.GetUtcOffset(start));
      var dateEnd = new DateTimeOffset(end, curTZone.GetUtcOffset(end));
      var startTimeString = dateStart.ToString("o");
      var endTimeString = dateEnd.ToString("o");
      var myEvent = new Event
      {
        Summary = "Noah, Laurens, Kevin en Sven are testing the overlapping appointments algorithm",
        Location = "Hogeschool Rotterdam",
        Description = "Working on Smart Doctor",
        Start = new EventDateTime
        {
          DateTime = Convert.ToDateTime(startTimeString),
          TimeZone = "GMT+2:00"
        },
        End = new EventDateTime
        {
          DateTime = Convert.ToDateTime(endTimeString),
          TimeZone = "GMT+2:00"
        },
        //Recurrence = new String[] { "RRULE:FREQ=ONCE;BYDAY=MO" },
        Attendees = new List<EventAttendee>
                {
                new EventAttendee { Email = "0929035@hr.nl" },
                new EventAttendee { Email =  "0925648@hr.nl" },
                new EventAttendee { Email =  "0930803@hr.nl" },
                new EventAttendee { Email =  "0923444@hr.nl" },
                },
      };

      if (overlapping == false)
      {
        var recurringEvent = service.Events.Insert(myEvent, "primary");
        recurringEvent.SendNotifications = true;
        recurringEvent.Execute();
      }

      if (overlapping == true)
      {
        Console.WriteLine("there is already an apppointment scheduled at the given date and time");
      }

      Console.Read();
    }*/
  }
}
