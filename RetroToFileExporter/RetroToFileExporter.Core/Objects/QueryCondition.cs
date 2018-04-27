using System;
using RSDU.Messaging;

namespace RetroToFileExporter.Core.Objects
{
    public class QueryCondition
    {
        public QueryCondition()
        {
            
        }
        public QueryCondition(string name, string directionTime, string interval, int intervalCount, int offset, string guidTrigger, string guid)
        {
            GuidQuery = new  Guid(guid);
            Name = name;
            DirectionTime = directionTime;
            Interval = interval;
            IntervalCount = intervalCount;
            Offset = offset;
            GuidTrigger = new Guid(guidTrigger);
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            try
            {
                var q = (QueryCondition) obj;
                if (q != null && GuidQuery == q.GuidQuery) return true;
            }
            catch (Exception ex)
            {
                Log.Error("Сравнение запросов выполнено с ошибкой ", ex);
                return false;
            }
            return false;
        }

        protected bool Equals(QueryCondition other)
        {
            return GuidQuery.Equals(other.GuidQuery) && string.Equals(Name, other.Name) && string.Equals(DirectionTime, other.DirectionTime) && string.Equals(Interval, other.Interval) && IntervalCount == other.IntervalCount && Offset == other.Offset && GuidTrigger.Equals(other.GuidTrigger);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = GuidQuery.GetHashCode();
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DirectionTime != null ? DirectionTime.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Interval != null ? Interval.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ IntervalCount;
                hashCode = (hashCode*397) ^ Offset;
                hashCode = (hashCode*397) ^ GuidTrigger.GetHashCode();
                return hashCode;
            }
        }

        public Guid GuidQuery { get; set; }
        public string Name { get; set; }
        public string DirectionTime { get; set; }
        public string Interval { get; set; }
        public int IntervalCount { get; set; }
        public int Offset { get; set; }
        public Guid GuidTrigger { get; set; }
    }
}