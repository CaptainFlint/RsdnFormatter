using System;
using System.Web;

namespace Rsdn.Framework.Formatting
{
	/// <summary>
	/// ������������� ������� ��������������.
	/// </summary>
	public static partial class Format
	{
		/// <summary>
		/// ������������� ������� �������������� ����.
		/// </summary>
		public class Date
		{
			private readonly DateTime _dateTimeValue;

			/// <summary>
			/// ����������� �������.
			/// </summary>
			public Date()
			{
				_dateTimeValue = DateTime.Now;
			}

			/// <summary>
			/// ����������� �������.
			/// </summary>
			/// <param name="dateTime">����.</param>
			public Date(DateTime dateTime)
			{
				_dateTimeValue = dateTime;
			}

			/// <summary>
			/// Get client time zone's offset in minutes from HttpContext
			/// </summary>
			/// <remarks>If HttpContext is absent, no offset is provided</remarks>
			/// <returns></returns>
			public static double GetClientTimeZoneOffset()
			{
				double timezoneOffsetMinutes = 0;
				if (HttpContext.Current != null)
				{
					var tzCookie = HttpContext.Current.Request.Cookies["tz"];
					double val;
					timezoneOffsetMinutes = 
						tzCookie != null && double.TryParse(tzCookie.Value, out val)
							? val
							: 0;
				}
				return timezoneOffsetMinutes;
			}

			/// <summary>
			/// Correct client time to server time.
			/// </summary>
			/// <param name="clientTime">Client time.</param>
			/// <returns>Server time.</returns>
			public static DateTime CorrectToServerTime(DateTime clientTime)
			{
				return clientTime.AddMinutes(-GetClientTimeZoneOffset()).ToLocalTime();
			}

			/// <summary>
			/// Correct server time to client timezone's time
			/// </summary>
			/// <param name="serverTime">Server time</param>
			/// <returns>Corrected time</returns>
			public static DateTime CorrectToClientTime(DateTime serverTime)
			{
				return Correct(serverTime, GetClientTimeZoneOffset());
			}

			/// <summary>
			/// Correct server time to client time zone's time
			/// </summary>
			/// <param name="serverTime">Server time</param>
			/// <param name="clientTimezoneOffsetMinutes">Client time zone's offset in minutes</param>
			/// <returns></returns>
			public static DateTime Correct(DateTime serverTime, double clientTimezoneOffsetMinutes)
			{
				return serverTime.ToUniversalTime().AddMinutes(clientTimezoneOffsetMinutes);
			}

			/// <summary>
			/// Correct server time to client time zone's time
			/// </summary>
			/// <param name="serverTime">Server time. If not DateTime then return zero date</param>
			/// <param name="clientTimezoneOffsetMinutes">Client time zone's offset in minutes</param>
			/// <returns></returns>
			public static DateTime Correct(object serverTime, double clientTimezoneOffsetMinutes)
			{
				return (serverTime is DateTime)
				       	?
				       		((DateTime) serverTime).ToUniversalTime().AddMinutes(clientTimezoneOffsetMinutes)
				       	:
				       		new DateTime(0);
			}

			/// <summary>
			/// ����������� ���� ��������� ������ "dd.MM.yy HH:mm"
			/// </summary>
			/// <returns>�������������� ������.</returns>
			public string ToYearString()
			{
				return ToYearString(_dateTimeValue);
			}

			/// <summary>
			/// ����������� ���� ��������� ������ "dd.MM.yy HH:mm"
			/// </summary>
			/// <param name="dateTime">������������� ����.</param>
			/// <returns>�������������� ������.</returns>
			public static string ToYearString(DateTime dateTime)
			{
				return dateTime.ToString("dd.MM.yy HH:mm");
			}

			/// <summary>
			/// ����������� ���� ��������� ������ "dd.MM.yy"
			/// </summary>
			/// <returns>�������������� ������.</returns>
			public string ToLongString()
			{
				return ToLongString(_dateTimeValue);
			}

			/// <summary>
			/// ����������� ���� ��������� ������ "dd.MM.yy"
			/// </summary>
			/// <param name="dateTime">������������� ����.</param>
			/// <returns>�������������� ������.</returns>
			public static string ToLongString(DateTime dateTime)
			{
				return dateTime.ToString("dd.MM.yy");
			}

			/// <summary>
			/// ����������� ���� ��������� ������ "dd.MM HH:mm"
			/// </summary>
			/// <returns>�������������� ������.</returns>
			public string ToShortString()
			{
				return ToShortString(_dateTimeValue);
			}

			/// <summary>
			/// ����������� ���� ��������� ������ "dd.MM HH:mm"
			/// </summary>
			/// <param name="dateTime">������������� ����.</param>
			/// <returns>�������������� ������.</returns>
			public static string ToShortString(DateTime dateTime)
			{
				return dateTime.ToString("dd.MM HH:mm");
			}

			/// <summary>
			/// ����������� ���� � ����������� �� �� ��������.
			/// ������ �������� - "dd.MM.yy.", ������ - "dd/MM HH:mm"
			/// </summary>
			/// <returns>�������������� ������.</returns>
			public string ToDependString()
			{
				return ToDependString(_dateTimeValue);
			}

			/// <summary>
			/// ����������� ���� � ����������� �� �� ��������.
			/// ������ �������� - "dd.MM.yy.", ������ - "dd/MM HH:mm"
			/// </summary>
			/// <param name="dateTime">������������� ����.</param>
			/// <returns>�������������� ������.</returns>
			public static string ToDependString(DateTime dateTime)
			{
				return (dateTime > DateTime.Now.AddMonths(-6))
				       	?
				       		ToShortString(dateTime)
				       	: ToLongString(dateTime);
			}

			/// <summary>
			/// ���������� ���� �� ������ �������� ���.
			/// </summary>
			/// <returns>������ �������� ���.</returns>
			public static DateTime GetDayBeginning()
			{
				return GetDayBeginning(DateTime.Now);
			}

			/// <summary>
			/// ���������� ���� �� ������ ��������� ���.
			/// </summary>
			/// <param name="dateTime">�������� ����.</param>
			/// <returns>������ ���.</returns>
			public static DateTime GetDayBeginning(DateTime dateTime)
			{
				return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
			}

			/// <summary>
			/// ���������� ���� �� ������ �������� ������.
			/// </summary>
			/// <returns>������ ������.</returns>
			public static DateTime GetMonthBeginning()
			{
				return GetMonthBeginning(DateTime.Now);
			}

			/// <summary>
			/// ���������� ���� �� ������ ��������� ������.
			/// </summary>
			/// <param name="dateTime">�������� ����.</param>
			/// <returns>������ ������.</returns>
			public static DateTime GetMonthBeginning(DateTime dateTime)
			{
				return new DateTime(dateTime.Year, dateTime.Month, 1);
			}
		}

		/// <summary>
		/// Top level RSDN domain name
		/// </summary>
		public static string RsdnDomainName = "rsdn.ru";
	}
}