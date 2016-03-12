using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using autopay.transactor.bundle.codebase.dto;
using Cassandra;
using LZ4;

namespace autopay.transactor.bundle.codebase.handler
{
    public class BatchGateTransformer : ITransactionTransformer
    {
        public void PerformAction(TransactionObj obj)
        {
            obj.Narration = "12345/ Jan Salary / LANG";
        }
    }

    public class StandardAutoGateTransformer : ITransactionTransformer
    {
        public void PerformAction(TransactionObj obj)
        {
            obj.PinBlock = "1dfgdikfufiodfjdiufhd!23";
        }
    }

    public class RtsAutoGateTransformer : ITransactionTransformer
    {
        public Cluster Cluster { get; private set; }

        public void PerformAction(TransactionObj obj)
        {
            Cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();

            Console.WriteLine("Connected to cluster: " +
            Cluster.Metadata.ClusterName.ToString(CultureInfo.InvariantCulture));

            foreach (var host in Cluster.Metadata.AllHosts())
            {
                Console.WriteLine("Data Center: " + host.Datacenter + ", " +
                "Host: " + host.Address + ", " +
                "Rack: " + host.Rack);
            }
            obj.ReceivingInstitutionId = "62806111";
        }
    }
}
