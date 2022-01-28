const AuthUrl = "#authUrl";
const CopyUrl = "#copyUrlTooltip";
 
$(document).ready(function () {

    var copyUrlTooltip = $(CopyUrl);
    var authUrlInput = $(AuthUrl);

    copyUrlTooltip.tooltip({
        trigger: 'click',
        placement: 'top',
        title: 'copied',
        delay: { "show": 500, "hide": 100 }
    });

    copyUrlTooltip.click(function (_) {
        clipboard.copy(authUrlInput.val());
    });

});