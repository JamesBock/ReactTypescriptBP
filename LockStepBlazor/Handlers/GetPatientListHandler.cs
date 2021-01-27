using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using LockStepBlazor.Application.Fhir.Queries;
using ReactTypescriptBP.Models;

namespace LockStepBlazor.Handlers
{
    public class GetPatientListHandler : GetPatientList.IHandler
    {
        private readonly FhirClient client;

        public GetPatientListHandler(FhirClient client)
        {
            this.client = client;

        }

        public async Task<GetPatientList.Model> Handle(GetPatientList.Query request, CancellationToken cancellationToken)
        {
            var paramsList = new List<string>();
            var queryNames = new SearchParams();
            if (!string.IsNullOrEmpty(request.FirstName))
            {
                request.FirstName = request.FirstName?.ToLower();
                request.FirstName = request.FirstName?.Trim();

                queryNames.Where($"given={request.FirstName}").SummaryOnly(SummaryType.Text);
                paramsList.Add($"given:contains={request.FirstName}");
                // queryNames.Where($"given={request.FirstName}").SummaryOnly(SummaryType.Count);
                //use this for a execute batch but for Medicaions
            }

            // if (!string.IsNullOrEmpty(request.LastName))
            // {

            //     request.LastName = request.LastName?.ToLower();
            //     request.LastName = request.LastName?.Trim();

            //     queryNames.Where($"family={request.LastName}").SummaryOnly(SummaryType.Text);
            //     //var p = queryNames.ToParameters().;
            //     paramsList.Add($"name:contains={request.LastName}");
            // }

            // var trans = new TransactionBuilder(client.Endpoint);
            // trans = trans.Search(q, "MedicationRequest").Search(t, "MedicationStatement");
            // //var result = await _client.SearchAsync<MedicationRequest>(q);
            // var tResult = _client.TransactionAsync(trans.ToBundle());

            // var result =
            //     PatientList
            //     .Where(x =>
            //         x.GivenNames.Select(n=> n.ToLower()).Contains(term) ||
            //         x.LastName.ToLower().Contains(term)
            //     )
            //     .ToList();
           
             //these shpuld be identical
            // var pats = await client.SearchAsync<Patient>(paramsList.ToArray(), default, 10/*, SummaryType.Text*/);
            var pats = await client.SearchAsync<Patient>(queryNames.LimitTo(10));

            var patientList = new List<PatientModel>();

            foreach (var e in pats.Entry)
            {
               
                Patient p = (Hl7.Fhir.Model.Patient)e.Resource;
                //var meds = PatientService.GetMedicationRequestsAsync(p.Id).GetAwaiter().GetResult();
                PatientModel pm = new PatientModel(p.Id, string.Join(" ",p.Name.FirstOrDefault().Given),string.Join(" ",p.Name.FirstOrDefault().Family));
                // patient.Medications = meds.Requests;
                patientList.Add(pm);
            }
            var listModel = new PatientListModel(patientList,
            pats.FirstLink?.ToString() != null ? pats.FirstLink.ToString() :"",
            pats.LastLink?.ToString() != null ? pats.LastLink.ToString() :"",
            pats.NextLink?.ToString() != null ? pats.NextLink.ToString() :"",
            pats.PreviousLink?.ToString() != null ? pats.PreviousLink.ToString() :""
            );

            return new GetPatientList.Model()
            {
               Payload = listModel
            };



        }


        // var queryNames = new SearchParams()
        //        .LimitTo(10);
        // var searchCall = await client.SearchAsync<Patient>(queryNames);


        // List<Patient> patients = new List<Patient>();
        // var res = new GetPatientList.Model();
        // // while (searchCall != null)
        // // {
        //     foreach (var e in searchCall.Entry)
        //     {
        //         Patient p = (Patient)e.Resource;
        //         patients.Add(p);
        //     }
        //     //TODO: How is this used?
        // //     searchCall = client.Continue(searchCall, PageDirection.Next);
        // // }

        // res.Requests = patients;
        // return res;
    }

}

