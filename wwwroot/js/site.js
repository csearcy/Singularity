$(document).ready(function() {
    function toggleFooterLinks() {
        if ($('.show-footer3').length > 0) {
            $('#footer3').show();
        } else {
            $('#footer3').hide();
        }
    }

    toggleFooterLinks();
    
    $('.nav-link').on('click', function(e) {
        e.preventDefault();
        var url = $(this).attr('href');

        $('#main-content').load(url + ' #main-content', function() {
            toggleFooterLinks();
        });
    });    
});
