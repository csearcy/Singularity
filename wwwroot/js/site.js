$(document).ready(function() {
    var dataIsReady = false;

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
    
    function checkDataReady() {
        if (typeof requiresData !== 'undefined' && requiresData && !dataIsReady) {
            $.ajax({
                url: '/api/data/status',
                method: 'GET',
                success: function(response) {
                    if (response.isReady) {
                        dataIsReady = true;
                        $('#main-content').load(window.location.href + ' #main-content');
                    } else {
                        $('#main-content').html('<p class="data-loading">Data is loading, please wait...</p>');
                        setTimeout(checkDataReady, 3000);
                    }
                }
            });
        }
    }

    checkDataReady();
});
