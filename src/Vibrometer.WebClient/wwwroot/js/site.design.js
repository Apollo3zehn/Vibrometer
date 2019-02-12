var isMobileMenuVisible = false;

isWindows = navigator.platform.indexOf('Win') > -1 ? true : false;

if (isWindows)
{
    $('.sidebar .sidebar-wrapper, .main-panel').perfectScrollbar();

    $('html').addClass('perfect-scrollbar-on');
} else
{
    $('html').addClass('perfect-scrollbar-off');
}

$(document).ready(function ()
{
    $('body').bootstrapMaterialDesign();

    $sidebar = $('.sidebar');

    window_width = $(window).width();

    Design.checkSidebarImage();
    Design.initMinimizeSidebar();
});

$(document).on('click', '.navbar-toggler', function ()
{
    $toggle = $(this);

    if (isMobileMenuVisible)
    {
        $('html').removeClass('nav-open');

        $('.close-layer').remove();
        setTimeout(function ()
        {
            $toggle.removeClass('toggled');
        }, 400);

        isMobileMenuVisible = false;
    } else
    {
        setTimeout(function ()
        {
            $toggle.addClass('toggled');
        }, 430);

        var $layer = $('<div class="close-layer"></div>');

        if ($('body').find('.main-panel').length !== 0)
        {
            $layer.appendTo(".main-panel");

        } else if ($('body').hasClass('off-canvas-sidebar'))
        {
            $layer.appendTo(".wrapper-full-page");
        }

        setTimeout(function ()
        {
            $layer.addClass('visible');
        }, 100);

        $layer.click(function ()
        {
            $('html').removeClass('nav-open');
            isMobileMenuVisible = 0;

            $layer.removeClass('visible');

            setTimeout(function ()
            {
                $layer.remove();
                $toggle.removeClass('toggled');

            }, 400);
        });

        $('html').addClass('nav-open');
        isMobileMenuVisible = 1;
    }
});

Design = {
    settings: {
        sidebar_mini_active: false
    },

    checkSidebarImage: function ()
    {
        $sidebar = $('.sidebar');
        image_src = $sidebar.data('image');

        if (image_src !== undefined)
        {
            sidebar_container = '<div class="sidebar-background" style="background-image: url(' + image_src + ') "/>';
            $sidebar.append(sidebar_container);
        }
    },

    initMinimizeSidebar: function ()
    {
        $('#minimizeSidebar').click(function ()
        {
            if (Design.settings.sidebar_mini_active)
            {
                $('body').removeClass('sidebar-mini');
                Design.settings.sidebar_mini_active = false;
            } else
            {
                $('body').addClass('sidebar-mini');
                Design.settings.sidebar_mini_active = true;
            }
        });
    }
};