namespace Glitch9.IO.Network
{
    public class ScheduleConverter : CloudConverter<Schedule>
    {
        public override Schedule ToLocalFormat(string propertyName, object propertyValue)
        {
            string stringValue = CloudConverterUtils.SafeConvertToString(propertyValue);
            return stringValue == null ? new Schedule() : Schedule.Create(stringValue);
        }

        public override object ToCloudFormat(Schedule propertyValue)
        {
            return propertyValue.ToString();
        }
    }
}