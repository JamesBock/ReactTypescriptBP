using Hl7.Fhir.Model;
using MediatR;
using ReactTypescriptBP.Models;
using System;
using System.Collections.Generic;


namespace LockStepBlazor.Application.Fhir.Queries
{
    public class GetPatientList
    {
        public class Query : IRequest<Model>
        {
            public string FirstName { get; set; }
            // public string LastName { get; set; }
        }

        public class Model
        {
            public PatientListModel Payload { get; set; }
        
        }

        public interface IHandler : IRequestHandler<Query, Model>
        {

        }
    }
}