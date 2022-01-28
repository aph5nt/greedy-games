$('tr[data-href]').on("click",
    function() {
        window.open($(this).data('href'), '_blank');
    });