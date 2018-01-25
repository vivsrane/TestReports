<%@ Control Language="C#" AutoEventWireup="true" Inherits="BV.Controls.Controls_Reports_DatePicker" Codebehind="DatePicker.ascx.cs" %>
<label >From :</label>
<asp:TextBox ID="from" CssClass ="fromDate" runat="server" Width="80px" ReadOnly = "true" ></asp:TextBox>
<label >  To :</label>
<asp:TextBox ID="to" CssClass ="toDate" runat="server"  Width="80px"  ReadOnly = "true" ></asp:TextBox>
<link href="Static/jquery-ui.css" rel="stylesheet" type="text/css" />
<script src="Static/Scripts/jquery-1.10.2.js" type="text/javascript"></script>
<script src="Static/Scripts/jquery-ui.js" type="text/javascript"></script>

<script type="text/javascript">
      var jquery_1_10_2 = $.noConflict(true);
</script>
  <script type="text/javascript">
      jquery_1_10_2(function () {
          jquery_1_10_2(".fromDate").datepicker({
              defaultDate: "+1w",
              changeMonth: true,
              numberOfMonths: 3,
              onClose: function (selectedDate) {
                  jquery_1_10_2(".toDate").datepicker("option", "minDate", selectedDate);
              }
          });
          jquery_1_10_2(".toDate").datepicker({
              defaultDate: "+1w",
              changeMonth: true,
              numberOfMonths: 3,
              onClose: function (selectedDate) {
                  jquery_1_10_2(".fromDate").datepicker("option", "maxDate", selectedDate);
              }
          });
      });
  </script>