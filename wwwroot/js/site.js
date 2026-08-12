/* ═══════════════════════════════════════════════════════════════════════════
   SVEI — front-end behaviour (vanilla, no framework)
   ═══════════════════════════════════════════════════════════════════════════ */
(function () {
  'use strict';

  const $  = (s, c = document) => c.querySelector(s);
  const $$ = (s, c = document) => Array.from(c.querySelectorAll(s));

  document.addEventListener('DOMContentLoaded', () => {
    stickyHeader();
    mobileDrawer();
    scrollReveal();
    statCounters();
    accordions();
    backToTop();
    heroSlider();
    brandSlider();
    lightbox();
    contactMap();
    videoEmbeds();
    newsletterForm();
    formGuards();
  });

  /* ── Sticky header shadow ─────────────────────────────────────────────── */
  function stickyHeader() {
    const header = $('.sv-header');
    if (!header) return;
    const onScroll = () => header.classList.toggle('is-stuck', window.scrollY > 8);
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });
  }

  /* ── Mobile drawer ────────────────────────────────────────────────────── */
  function mobileDrawer() {
    const drawer = $('.sv-drawer');
    const burger = $('.sv-burger');
    if (!drawer || !burger) return;

    const open  = () => { drawer.classList.add('is-open'); document.body.style.overflow = 'hidden'; burger.setAttribute('aria-expanded', 'true'); };
    const close = () => { drawer.classList.remove('is-open'); document.body.style.overflow = ''; burger.setAttribute('aria-expanded', 'false'); };

    burger.addEventListener('click', open);
    $$('.sv-drawer__close, .sv-drawer__overlay', drawer).forEach(el => el.addEventListener('click', close));
    $$('.sv-drawer__nav a', drawer).forEach(a => a.addEventListener('click', close));
    document.addEventListener('keydown', e => { if (e.key === 'Escape') close(); });
  }

  /* ── Scroll reveal ────────────────────────────────────────────────────── */
  function scrollReveal() {
    const items = $$('.sv-reveal');
    if (!items.length) return;

    if (!('IntersectionObserver' in window)) {
      items.forEach(el => el.classList.add('is-visible'));
      return;
    }
    const io = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('is-visible');
          io.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

    items.forEach(el => io.observe(el));
  }

  /* ── Animated stat counters ───────────────────────────────────────────── */
  function statCounters() {
    const nums = $$('[data-count]');
    if (!nums.length) return;

    const animate = (el) => {
      const target   = parseFloat(el.dataset.count) || 0;
      const decimals = parseInt(el.dataset.decimals || '0', 10);
      const prefix   = el.dataset.prefix || '';
      const suffix   = el.dataset.suffix || '';
      const duration = 1600;
      const start    = performance.now();

      const tick = (now) => {
        const p = Math.min((now - start) / duration, 1);
        // easeOutExpo
        const eased = p === 1 ? 1 : 1 - Math.pow(2, -10 * p);
        const value = (target * eased).toFixed(decimals);
        el.textContent = prefix + Number(value).toLocaleString(document.documentElement.lang || 'en') + suffix;
        if (p < 1) requestAnimationFrame(tick);
      };
      requestAnimationFrame(tick);
    };

    if (!('IntersectionObserver' in window)) { nums.forEach(animate); return; }
    const io = new IntersectionObserver((entries) => {
      entries.forEach(e => { if (e.isIntersecting) { animate(e.target); io.unobserve(e.target); } });
    }, { threshold: 0.5 });
    nums.forEach(el => io.observe(el));
  }

  /* ── Accordion ────────────────────────────────────────────────────────── */
  function accordions() {
    $$('.sv-acc').forEach(acc => {
      const items = $$('.sv-acc__item', acc);
      items.forEach(item => {
        const head = $('.sv-acc__head', item);
        const body = $('.sv-acc__body', item);
        if (!head || !body) return;

        head.setAttribute('aria-expanded', 'false');
        head.addEventListener('click', () => {
          const isOpen = item.classList.contains('is-open');
          if (!acc.dataset.multi) {
            items.forEach(o => {
              o.classList.remove('is-open');
              const b = $('.sv-acc__body', o); if (b) b.style.maxHeight = null;
              const h = $('.sv-acc__head', o); if (h) h.setAttribute('aria-expanded', 'false');
            });
          }
          if (!isOpen) {
            item.classList.add('is-open');
            body.style.maxHeight = body.scrollHeight + 'px';
            head.setAttribute('aria-expanded', 'true');
          }
        });
      });
    });
  }

  /* ── Back to top ──────────────────────────────────────────────────────── */
  function backToTop() {
    const btn = $('.sv-totop');
    if (!btn) return;
    const onScroll = () => btn.classList.toggle('is-visible', window.scrollY > 500);
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });
    btn.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));
  }

  /* ── Hero slider (Swiper) ─────────────────────────────────────────────── */
  function heroSlider() {
    const hero = $('.sv-hero');
    const el   = $('.sv-hero .swiper');
    if (!hero || !el) return;

    const count = el.querySelectorAll('.swiper-slide').length;

    // Mark as ready so the CSS can reveal the slides. Without this the hero
    // stayed hidden (or flickered) whenever Swiper bailed out or was slow.
    const ready = () => hero.classList.add('is-ready');

    if (typeof Swiper === 'undefined' || count < 2) { ready(); return; }

    new Swiper(el, {
      // With only 2 slides, loop + fade makes Swiper duplicate them and the
      // crossfade visibly blinks. Rewind gives a clean A->B->A cycle instead.
      loop: count > 2,
      rewind: count === 2,
      speed: 900,
      effect: 'fade',
      fadeEffect: { crossFade: true },
      autoplay: { delay: 6500, disableOnInteraction: false, pauseOnMouseEnter: true },
      pagination: { el: '.sv-hero .swiper-pagination', clickable: true },
      a11y: { enabled: true },
      on: { init: ready }
    });
  }

  /* ── Brand marquee (Swiper) ───────────────────────────────────────────── */
  function brandSlider() {
    const wrap = $('.sv-brands');
    const el   = $('.sv-brands .swiper');
    if (!wrap || !el) return;

    const slides = el.querySelectorAll('.swiper-slide');

    // Swiper's loop mode needs noticeably more slides than are on screen.
    // With only a handful of logos it duplicates/misplaces them, which is what
    // stacked UNITRONICS on top of the row. Below that threshold, skip Swiper
    // entirely and render a centred static strip instead.
    const perView = 5;
    if (typeof Swiper === 'undefined' || slides.length <= perView) {
      wrap.classList.add('sv-brands--static');
      return;
    }

    new Swiper(el, {
      loop: true,
      loopAdditionalSlides: slides.length,
      slidesPerView: 2,
      spaceBetween: 32,
      speed: 4000,
      autoplay: { delay: 0, disableOnInteraction: false, pauseOnMouseEnter: true },
      freeMode: true,
      allowTouchMove: false,
      breakpoints: { 640: { slidesPerView: 3 }, 900: { slidesPerView: 4 }, 1200: { slidesPerView: perView } }
    });
  }

  /* ── Lightbox (GLightbox) ─────────────────────────────────────────────── */
  function lightbox() {
    if (typeof GLightbox === 'undefined') return;
    if (!$('.glightbox')) return;
    GLightbox({ selector: '.glightbox', touchNavigation: true, loop: true });
  }

  /* ── Contact map (Leaflet + OpenStreetMap, no API key) ────────────────── */
  function contactMap() {
    const el = $('#sv-map');
    if (!el || typeof L === 'undefined') return;

    const lat  = parseFloat(el.dataset.lat) || 29.6725549;
    const lng  = parseFloat(el.dataset.lng) || 32.30928;
    const zoom = parseInt(el.dataset.zoom || '13', 10);
    const label = el.dataset.label || 'SVEI';

    const map = L.map(el, { scrollWheelZoom: false }).setView([lat, lng], zoom);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    const icon = L.divIcon({
      className: 'sv-map-pin',
      html: '<span style="display:grid;place-items:center;width:34px;height:34px;background:#E31B23;color:#fff;border-radius:50% 50% 50% 0;transform:rotate(-45deg);box-shadow:0 4px 12px rgba(0,0,0,.3)"><i class="fa-solid fa-industry" style="transform:rotate(45deg)"></i></span>',
      iconSize: [34, 34],
      iconAnchor: [17, 34]
    });
    L.marker([lat, lng], { icon }).addTo(map).bindPopup('<strong>' + label + '</strong>');

    map.on('click', () => map.scrollWheelZoom.enable());
    map.on('mouseout', () => map.scrollWheelZoom.disable());
  }

  /* ── Click-to-load YouTube (keeps the page fast) ──────────────────────── */
  function videoEmbeds() {
    $$('[data-video]').forEach(box => {
      const btn = $('.sv-video__play', box);
      if (!btn) return;
      btn.addEventListener('click', () => {
        const id = extractYouTubeId(box.dataset.video);
        if (!id) { window.open(box.dataset.video, '_blank', 'noopener'); return; }
        box.innerHTML =
          '<iframe style="width:100%;height:100%;border:0" allowfullscreen ' +
          'allow="accelerometer;autoplay;clipboard-write;encrypted-media;gyroscope;picture-in-picture" ' +
          'src="https://www.youtube-nocookie.com/embed/' + id + '?autoplay=1&rel=0"></iframe>';
      });
    });
  }

  function extractYouTubeId(url) {
    if (!url) return null;
    const m = url.match(/(?:youtu\.be\/|v=|embed\/|shorts\/)([A-Za-z0-9_-]{11})/);
    return m ? m[1] : null;
  }

  /* ── Newsletter (AJAX, no page reload) ────────────────────────────────── */
  function newsletterForm() {
    $$('form[data-newsletter]').forEach(form => {
      form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const btn = $('button[type=submit]', form);
        const note = $('.sv-newsletter__note', form.parentElement) || null;
        if (btn) { btn.disabled = true; btn.classList.add('is-loading'); }

        try {
          const res  = await fetch(form.action, { method: 'POST', body: new FormData(form) });
          const data = await res.json();
          if (note) {
            note.textContent = data.message || '';
            note.style.color = data.success ? '#22C6E8' : '#FF3B43';
          }
          if (data.success) form.reset();
        } catch {
          if (note) note.textContent = '⚠';
        } finally {
          if (btn) { btn.disabled = false; btn.classList.remove('is-loading'); }
        }
      });
    });
  }

  /* ── Prevent double submits + show file names ─────────────────────────── */
  function formGuards() {
    $$('form:not([data-newsletter])').forEach(form => {
      form.addEventListener('submit', () => {
        const btn = $('button[type=submit]', form);
        if (btn && form.checkValidity()) {
          setTimeout(() => { btn.disabled = true; btn.classList.add('is-loading'); }, 0);
        }
      });
    });

    $$('input[type=file]').forEach(input => {
      input.addEventListener('change', () => {
        const label = input.closest('.sv-field')?.querySelector('.sv-file__name');
        if (label) label.textContent = input.files?.[0]?.name || '';
      });
    });
  }
})();
