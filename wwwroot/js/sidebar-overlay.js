window.etrsSidebar = {
  showProfileOverlay: function (btn, overlay, show) {
    try {
      // Resolve the button element - Blazor ElementReference may not always arrive as a direct DOM node
      var btnEl = btn;
      if (!btnEl || typeof btnEl.getBoundingClientRect !== 'function') {
        try {
          if (btn && typeof btn === 'object' && btn.id) {
            btnEl = document.getElementById(btn.id) || btnEl;
          }
        } catch (e) { /* ignore */ }
      }
      // fallback selector
      if (!btnEl || typeof btnEl.getBoundingClientRect !== 'function') {
        btnEl = document.querySelector('.etrs-profile-actions');
      }

      if (!btnEl || typeof btnEl.getBoundingClientRect !== 'function') {
        console.warn('etrsSidebar.showProfileOverlay: unable to resolve button element', btn);
        return;
      }

      var btnRect = btnEl.getBoundingClientRect();

      // Resolve overlay element similarly
      var overlayEl = overlay;
      if (!overlayEl || typeof overlayEl.style === 'undefined') {
        try {
          if (overlay && typeof overlay === 'object' && overlay.id) {
            overlayEl = document.getElementById(overlay.id) || overlayEl;
          }
        } catch (e) { /* ignore */ }
      }
      if (!overlayEl || typeof overlayEl.style === 'undefined') {
        overlayEl = document.querySelector('.etrs-profile-overlay');
      }
      if (!overlayEl) {
        console.warn('etrsSidebar.showProfileOverlay: unable to resolve overlay element', overlay);
        return;
      }

      // helper storage on element to remember placeholder when moved to body
      if (!overlayEl.__etrs) overlayEl.__etrs = {};

      if (show) {
        // move overlay to document.body so it won't be clipped by sidebar/container
        if (overlayEl.parentNode !== document.body) {
          // create placeholder comment node to restore later
          var placeholder = document.createComment('etrs-overlay-placeholder');
          overlayEl.__etrs.placeholder = placeholder;
          overlayEl.parentNode.insertBefore(placeholder, overlayEl);
          document.body.appendChild(overlayEl);
        }

        overlayEl.style.display = 'flex';
        overlayEl.style.position = 'fixed';
        overlayEl.style.right = 'auto';

        // Defer position calculations until after the overlay is rendered so offsetWidth/offsetHeight are correct
        requestAnimationFrame(function () {
          // recompute button rect now that layout may have changed (overlay moved to body)
          try { btnRect = btnEl.getBoundingClientRect(); } catch (e) { /* ignore */ }
          // Try anchoring the menu to the top-right area of the sidebar (so it appears in the highlighted space)
          var left = null;
          var top = null;
          var sbRect = null;

          try {
            var sidebarEl = btnEl.closest && btnEl.closest('.etrs-sidebar');
            if (!sidebarEl) sidebarEl = document.querySelector('.etrs-sidebar');
            if (sidebarEl && sidebarEl.getBoundingClientRect) {
              sbRect = sidebarEl.getBoundingClientRect();
            // place overlay to the right of the sidebar
            left = Math.round(sbRect.right + 12);
            try {
              // small horizontal nudge to shift the overlay slightly left
              var nudge = 12;
              if (typeof overlayEl.offsetHeight === 'number' && typeof overlayEl.offsetWidth === 'number') {
                var h = overlayEl.offsetHeight;
                var w = overlayEl.offsetWidth;
                // center vertically on the profile button
                var centeredTop = Math.round(btnRect.top + (btnRect.height - h) / 2);
                var minTop = Math.round(sbRect.top + 24);
                var maxTop = Math.round(sbRect.bottom - h - 20);
                if (maxTop < minTop) maxTop = minTop;
                top = Math.max(minTop, Math.min(centeredTop, maxTop));

                // if overlay would overflow the right edge, try positioning to the left of the sidebar
                if (left + w > window.innerWidth) {
                  left = Math.round(sbRect.left - w - 12 + nudge);
                }
                // apply nudge to shift overlay slightly left when on the right side
                left = Math.round(left - nudge);
              } else {
                top = Math.round(sbRect.top + 56);
              }
            } catch (e) { /* ignore and fall back */ }
            }
          } catch (e) { /* ignore and fall back */ }

          // fallback to anchoring near the button if we couldn't position by sidebar
          if (left === null || top === null) {
            // Anchor: align the bottom-left corner of the menu to the 3-dot button
            // Bottom-left of menu -> (btnRect.right, btnRect.bottom)
            left = Math.round(btnRect.right);
            top = Math.round(btnRect.bottom - overlayEl.offsetHeight);
          }

          // (kept) do not override the previously computed top so overlay stays aligned with the profile button

          // If the menu would overflow the right edge, adjust to fit inside viewport
          if (left + overlayEl.offsetWidth > window.innerWidth) {
            // prefer moving leftwards so it still sits near the sidebar if possible
            if (sbRect) {
              left = Math.max(8, window.innerWidth - overlayEl.offsetWidth - 8);
            } else {
              left = Math.round(btnRect.right - overlayEl.offsetWidth);
            }
          }

          // Clamp to viewport vertically/horizontally
          left = Math.max(8, Math.min(left, window.innerWidth - overlayEl.offsetWidth - 8));
          top = Math.max(8, Math.min(top, window.innerHeight - overlayEl.offsetHeight - 8));

          overlayEl.style.left = left + 'px';
          overlayEl.style.top = top + 'px';

          // animate in
          overlayEl.style.opacity = '0';
          overlayEl.style.transform = 'translateY(6px)';
          setTimeout(function () {
            overlayEl.style.transition = 'opacity 160ms ease, transform 160ms ease';
            overlayEl.style.opacity = '1';
            overlayEl.style.transform = 'translateY(0)';
          }, 10);
        });

        // mark shown and attach global listeners to hide when clicking outside or on scroll
        overlayEl.__etrs.isShown = true;
        // remove existing handlers if any
        if (overlayEl.__etrs.onDocClick) document.removeEventListener('mousedown', overlayEl.__etrs.onDocClick, true);
        if (overlayEl.__etrs.onScroll) window.removeEventListener('scroll', overlayEl.__etrs.onScroll, true);

        overlayEl.__etrs.onDocClick = function (ev) {
          try {
            var t = ev.target;
            if (!overlayEl.contains(t) && !btnEl.contains(t)) {
              window.etrsSidebar.showProfileOverlay(btnEl, overlayEl, false);
            }
          } catch (e) { }
        };

        overlayEl.__etrs.onScroll = function () {
          try { window.etrsSidebar.showProfileOverlay(btnEl, overlayEl, false); } catch (e) { }
        };

        document.addEventListener('mousedown', overlayEl.__etrs.onDocClick, true);
        window.addEventListener('scroll', overlayEl.__etrs.onScroll, true);
      } else {
        // animate out then hide and restore to original parent
        overlayEl.style.transition = 'opacity 120ms ease, transform 120ms ease';
        overlayEl.style.opacity = '0';
        overlayEl.style.transform = 'translateY(6px)';
        setTimeout(function () {
          overlayEl.style.display = 'none';
          // restore to original place if we moved it
          try {
            var ph = overlayEl.__etrs && overlayEl.__etrs.placeholder;
            if (ph && ph.parentNode) {
              ph.parentNode.insertBefore(overlayEl, ph);
              ph.parentNode.removeChild(ph);
              overlayEl.__etrs.placeholder = null;
            }
          } catch (e) { console.warn(e); }
          // remove global handlers
          try {
            if (overlayEl.__etrs && overlayEl.__etrs.onDocClick) {
              document.removeEventListener('mousedown', overlayEl.__etrs.onDocClick, true);
              overlayEl.__etrs.onDocClick = null;
            }
            if (overlayEl.__etrs && overlayEl.__etrs.onScroll) {
              window.removeEventListener('scroll', overlayEl.__etrs.onScroll, true);
              overlayEl.__etrs.onScroll = null;
            }
            overlayEl.__etrs.isShown = false;
          } catch (e) { }
        }, 160);
      }
    } catch (e) {
      console.warn(e);
    }
  }
};
