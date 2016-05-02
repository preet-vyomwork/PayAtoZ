// Initialize this and store globally for tracking state.
var currentPage = 1;

// Initialize this to 1, so that "Next" is disabled until
//  GetItemCount returns and we know there's a second page.
var lastPage = 1;

// Set this to any integer you like. 5-7 works well
//  with the FeedBurner data source.
var pageSize = 9;

$(document).ready(function() {
  // On page load, display the first page of results.
  //DisplayRSSTable(1);
  
  // Simultaneously, begin loading the total item count.
  //GetRSSItemCount();
});

function DisplayRSSTable(page) {
  $.ajax({
    type: "POST",
    url: "/ConfirmOrder/GetCouponsList",
    data: "{'PageSize':'" + pageSize + "', 'Page':'" + page + "'}",
    contentType: "application/json; charset=utf-8",
    //dataType: "json",
    success: function (data) {
        debugger;
        var div = $("#dvCoupens");
        div.html(data);
    },
    error: function () {
        alert('div not found');
      }
  });
}

function GetRSSItemCount() {
  $.ajax({
    type: "POST",
    url: "Default.aspx/GetFeedBurnerItemCount",
    data: "{}",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function(msg) {
      // msg.d will contain the total number of items, as
      //  an integer. Divide to find total number of pages.
      lastPage = Math.ceil(msg.d / pageSize);

      // Wireup appropriate paging functionality.
      UpdatePaging();
    }
  });
}

function DisplayProgressIndication() {
  // Hide both of the paging controls,
  //  to avoid click-happy users.
  $('.paging').hide();

  // Clean up our event handlers, to avoid memory leaks.
  $('.paging').unbind();

  // Store the height of the content area of the table.
  var height = $('#RSSTable tbody').height();

  // Replace the entire content area with a single row/cell.
  $('#RSSTable tbody').html('<tr><td colspan="2"></td></tr>');
  
  // Set that row's height to be the same as previous.
  $('#RSSTable tbody tr').height(height);
  
  // Add our centered progress indicator animation to it.
  $('#RSSTable tbody td').addClass('loading');
}

function ApplyTemplate(msg) {
  // Changed the template extension from .tpl to .htm, 
  //  to avoid the request being blocked by some IIS installs.
  $('#Container').setTemplateURL('RSSTable.htm',
                                 null, { filter_data: false });
  $('#Container').processTemplate(msg);
}

function UpdatePaging() {
  // If we're not on the first page, enable the "Previous" link.
  if (currentPage != 1) {
    $('#PrevPage').attr('href', '#');
    $('#PrevPage').click(PrevPage);
  }

  // If we're not on the last page, enable the "Next" link.
  if (currentPage != lastPage) {
    $('#NextPage').attr('href', '#');
    $('#NextPage').click(NextPage);
  }
}

function NextPage(evt) {
  // Prevent the browser from navigating needlessly to #.
  //evt.preventDefault();

  // Entertain the user while the previous page is loaded.
  DisplayProgressIndication();

  // Load and render the next page of results, and
  //  increment the current page number.
  DisplayRSSTable(++currentPage);
}

function PrevPage(evt) {
  // Prevent the browser from navigating needlessly to #.
  //evt.preventDefault();

  // Entertain the user while the previous page is loaded.
  DisplayProgressIndication();

  // Load and render the previous page of results, and
  //  decrement the current page number.
  DisplayRSSTable(--currentPage);
}