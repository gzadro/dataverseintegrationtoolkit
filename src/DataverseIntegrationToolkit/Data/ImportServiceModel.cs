using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseIntegrationToolkit.Data
{
    public class ImportServiceModel
    {
        public Entity Entity { get; set; }

        public Entity? ToEntity { get; set; }

        public Relationship? Relationship { get; set; }

        public Entity? PreviousState { get; set; }

        public Operation Operation { get; set; }

        public bool? IsSuccess { get; set; }

        public ImportServiceModel(Entity entity, Operation operation)
        {
            Entity = entity;
            Operation = operation;
        }

        public ImportServiceModel(Entity entity, Entity toEntity, Operation operation, Relationship relationship)
        {
            Entity = entity;
            ToEntity = toEntity;
            Operation = operation;
            Relationship = relationship;
        }
    }
}
