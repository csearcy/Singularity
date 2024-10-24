$(document).ready(function() {
    $('.nav-link').on('click', function(e) {
        e.preventDefault();
        var url = $(this).attr('href');
        
        $('#main-content').load(url + ' #main-content');
    });
});