using Entities.DTO;
using System.Globalization;

namespace Desktop.Converters;

public class TaskAssignedToUserConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length != 2)
            return Colors.Transparent;

        if (values[0] is not TaskGetDto task || values[1] is not Guid currentUserId)
            return Colors.Transparent;

        if (task.DueDate.HasValue && task.DueDate.Value.Date < DateTime.Today)
        {
            return Colors.Red;
        }

        bool isAssigned = task.Users.Any(u => u.UserId == currentUserId);
        
        return isAssigned ? Colors.Green : Colors.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
