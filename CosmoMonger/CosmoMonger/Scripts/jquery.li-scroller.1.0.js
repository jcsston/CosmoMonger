jQuery.fn.liScroll = function(settings) {
    settings = jQuery.extend({
        travelocity: 0.07
    }, settings);
    return this.each(function() {
        var $strip = jQuery(this);
        $strip.addClass("newsticker")
        var stripWidth = 0;
        var $mask = $strip.wrap("<div class='mask'></div>");
        var $tickercontainer = $strip.parent().wrap("<div class='tickercontainer'></div>");
        var containerWidth = $strip.parent().parent().width(); //a.k.a. 'mask' width 	
        $strip.find("li").each(function(i) {
            stripWidth += jQuery(this, i).width();
        });
        $strip.width(stripWidth);
        var defTiming = stripWidth / settings.travelocity;
        console.log(defTiming);
        var totalTravel = stripWidth + containerWidth;
        function scrollnews(spazio, tempo) {
            $strip.animate({ left: '-=' + spazio }, tempo, "linear", function() {
                $strip.css("left", containerWidth);
                scrollnews(totalTravel, defTiming);
            });
        }
        scrollnews(totalTravel, defTiming);
        function hoverNewsStart() {
            var offset = jQuery(this).offset();
            var residualSpace = offset.left + stripWidth;
            var residualTime = residualSpace / settings.travelocity;
            console.log(residualTime);
            scrollnews(residualSpace, residualTime);
        }
        $strip.hover(function() {
            jQuery(this).stop();
        }, hoverNewsStart);
    });
};