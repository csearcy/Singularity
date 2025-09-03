$(document).ready(function() {
    var dataIsReady = false;

    function toggleRightMenuLinks() {
        if ($('.show-RightMenu3').length > 0) {
            $('#RightMenu3').show();
        } else {
            $('#RightMenu3').hide();
        }
    }

    toggleRightMenuLinks();
    
    $('.nav-link').on('click', function(e) {
        e.preventDefault();
        var url = $(this).attr('href');

        $('#main-content').load(url + ' #main-content', function() {
            toggleRightMenuLinks();
        });
    });
    
    function checkDataReady() {
        if (typeof requiresData !== 'undefined' && requiresData && !dataIsReady) {
            $('body').addClass('loading-cursor');
           $.ajax({
                url: '/api/data/status',
                method: 'GET',
                success: function(response) {
                    if (response.isReady) {
                        dataIsReady = true;
                        $('#main-content').load(window.location.href + ' #main-content', function() {
                            $('.data-loading-overlay').remove();
                            $('body').removeClass('loading-cursor');
                        });
                    } else {
                        setTimeout(checkDataReady, 3000);
                    }
                },
                error: function() {
                    $('body').removeClass('loading-cursor');
                }
            });
        }
    }

    checkDataReady();
});

// Progress bar update function
function updateProgressBar(currentValue) {
        progressBar.value = currentValue; 
        if (currentValue === $0) {
            progressBar.classList.remove('progress-bar'); 
            progressBar.classList.add('progress-bar-empty'); 
        } else {
            progressBar.classList.remove('progress-bar-empty');
            progressBar.classList.add('progress-bar'); 
        }
    }

