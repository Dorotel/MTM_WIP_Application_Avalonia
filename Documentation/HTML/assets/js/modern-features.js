/**
 * MTM WIP Application - Modern Documentation Interactive Features
 * Documentation Modernization Initiative - Phase 3
 */

class MTMDocumentation {
  constructor() {
    this.init();
  }

  init() {
    this.setupThemeToggle();
    this.setupMobileNavigation();
    this.setupSearch();
    this.setupSmoothScrolling();
    this.setupProgressTracking();
    this.setupTooltips();
    this.setupKeyboardNavigation();
    this.setupAccessibility();
    this.animateElements();
  }

  /**
   * Theme Toggle Functionality
   */
  setupThemeToggle() {
    const themeToggle = document.querySelector('.theme-toggle');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)');
    
    // Initialize theme based on user preference or saved setting
    const savedTheme = localStorage.getItem('mtm-theme');
    const initialTheme = savedTheme || (prefersDark.matches ? 'dark' : 'light');
    this.setTheme(initialTheme);

    if (themeToggle) {
      themeToggle.addEventListener('click', () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        this.setTheme(newTheme);
        localStorage.setItem('mtm-theme', newTheme);
      });
    }

    // Listen for system theme changes
    prefersDark.addEventListener('change', (e) => {
      if (!localStorage.getItem('mtm-theme')) {
        this.setTheme(e.matches ? 'dark' : 'light');
      }
    });
  }

  setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    
    // Update theme toggle icon
    const themeToggle = document.querySelector('.theme-toggle');
    if (themeToggle) {
      themeToggle.innerHTML = theme === 'dark' ? '☀️' : '🌙';
      themeToggle.setAttribute('aria-label', `Switch to ${theme === 'dark' ? 'light' : 'dark'} theme`);
    }
  }

  /**
   * Mobile Navigation
   */
  setupMobileNavigation() {
    const sidebar = document.querySelector('.layout-sidebar');
    const content = document.querySelector('.layout-content');
    const menuToggle = document.querySelector('.menu-toggle');
    const overlay = document.createElement('div');
    
    overlay.className = 'mobile-overlay';
    overlay.style.cssText = `
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.5);
      z-index: 90;
      display: none;
      opacity: 0;
      transition: opacity 0.3s ease;
    `;
    document.body.appendChild(overlay);

    // Create mobile menu toggle if it doesn't exist
    if (!menuToggle && window.innerWidth <= 768) {
      const toggle = document.createElement('button');
      toggle.className = 'menu-toggle';
      toggle.innerHTML = '☰';
      toggle.style.cssText = `
        display: block;
        background: none;
        border: none;
        font-size: 1.5rem;
        cursor: pointer;
        color: var(--text-primary);
        padding: 0.5rem;
      `;
      toggle.setAttribute('aria-label', 'Toggle navigation menu');
      
      const header = document.querySelector('.header');
      if (header) {
        header.insertBefore(toggle, header.firstChild);
      }
      
      toggle.addEventListener('click', () => this.toggleMobileMenu());
    }

    // Close menu when clicking overlay
    overlay.addEventListener('click', () => this.closeMobileMenu());

    // Close menu when clicking nav links
    if (sidebar) {
      sidebar.addEventListener('click', (e) => {
        if (e.target.matches('.nav-link, .mtm-nav-link')) {
          this.closeMobileMenu();
        }
      });
    }

    // Handle resize
    window.addEventListener('resize', () => {
      if (window.innerWidth > 768) {
        this.closeMobileMenu();
      }
    });
  }

  toggleMobileMenu() {
    const sidebar = document.querySelector('.layout-sidebar');
    const overlay = document.querySelector('.mobile-overlay');
    
    if (sidebar && overlay) {
      const isOpen = sidebar.classList.contains('is-open');
      
      if (isOpen) {
        this.closeMobileMenu();
      } else {
        sidebar.classList.add('is-open');
        overlay.style.display = 'block';
        setTimeout(() => overlay.style.opacity = '1', 10);
        document.body.style.overflow = 'hidden';
      }
    }
  }

  closeMobileMenu() {
    const sidebar = document.querySelector('.layout-sidebar');
    const overlay = document.querySelector('.mobile-overlay');
    
    if (sidebar && overlay) {
      sidebar.classList.remove('is-open');
      overlay.style.opacity = '0';
      setTimeout(() => overlay.style.display = 'none', 300);
      document.body.style.overflow = '';
    }
  }

  /**
   * Search Functionality
   */
  setupSearch() {
    const searchContainer = document.querySelector('.search-container');
    if (!searchContainer) {
      this.createSearchBox();
    }

    const searchInput = document.querySelector('.search-input');
    const searchResults = document.querySelector('.search-results');

    if (searchInput) {
      let searchTimeout;
      
      searchInput.addEventListener('input', (e) => {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
          this.performSearch(e.target.value);
        }, 300);
      });

      // Close search results when clicking outside
      document.addEventListener('click', (e) => {
        if (!searchContainer?.contains(e.target)) {
          if (searchResults) {
            searchResults.style.display = 'none';
          }
        }
      });
    }
  }

  createSearchBox() {
    const header = document.querySelector('.header-actions');
    if (!header) return;

    const searchContainer = document.createElement('div');
    searchContainer.className = 'search-container';
    searchContainer.style.cssText = `
      position: relative;
      max-width: 300px;
      width: 100%;
    `;

    const searchInput = document.createElement('input');
    searchInput.type = 'text';
    searchInput.className = 'search-input';
    searchInput.placeholder = 'Search documentation...';
    searchInput.style.cssText = `
      width: 100%;
      padding: 0.5rem 1rem;
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
      background: var(--bg-primary);
      color: var(--text-primary);
      font-size: var(--font-size-sm);
    `;

    const searchResults = document.createElement('div');
    searchResults.className = 'search-results';
    searchResults.style.cssText = `
      position: absolute;
      top: 100%;
      left: 0;
      right: 0;
      background: var(--bg-primary);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-lg);
      max-height: 400px;
      overflow-y: auto;
      z-index: 1000;
      display: none;
      margin-top: 0.25rem;
    `;

    searchContainer.appendChild(searchInput);
    searchContainer.appendChild(searchResults);
    header.appendChild(searchContainer);
  }

  performSearch(query) {
    const searchResults = document.querySelector('.search-results');
    if (!searchResults || !query.trim()) {
      if (searchResults) searchResults.style.display = 'none';
      return;
    }

    // Simple search implementation - can be enhanced with indexing
    const searchableElements = document.querySelectorAll('h1, h2, h3, h4, h5, h6, p, li');
    const results = [];

    searchableElements.forEach(element => {
      const text = element.textContent.toLowerCase();
      if (text.includes(query.toLowerCase())) {
        const heading = this.findNearestHeading(element);
        results.push({
          element,
          heading: heading?.textContent || 'Documentation',
          text: text.substring(0, 150) + (text.length > 150 ? '...' : ''),
          url: window.location.pathname + (heading?.id ? `#${heading.id}` : '')
        });
      }
    });

    this.displaySearchResults(results.slice(0, 10)); // Limit to 10 results
  }

  findNearestHeading(element) {
    let current = element;
    while (current && current !== document.body) {
      if (current.matches('h1, h2, h3, h4, h5, h6')) {
        return current;
      }
      current = current.previousElementSibling || current.parentElement;
    }
    return null;
  }

  displaySearchResults(results) {
    const searchResults = document.querySelector('.search-results');
    if (!searchResults) return;

    if (results.length === 0) {
      searchResults.innerHTML = '<div style="padding: 1rem; color: var(--text-secondary);">No results found</div>';
    } else {
      searchResults.innerHTML = results.map(result => `
        <div style="padding: 0.75rem; border-bottom: 1px solid var(--border-color-light); cursor: pointer;"
             onclick="this.querySelector('a').click()">
          <a href="${result.url}" style="text-decoration: none; color: inherit;">
            <div style="font-weight: 500; color: var(--mtm-primary); margin-bottom: 0.25rem;">
              ${result.heading}
            </div>
            <div style="font-size: var(--font-size-sm); color: var(--text-secondary);">
              ${result.text}
            </div>
          </a>
        </div>
      `).join('');
    }

    searchResults.style.display = 'block';
  }

  /**
   * Smooth Scrolling
   */
  setupSmoothScrolling() {
    document.addEventListener('click', (e) => {
      if (e.target.matches('a[href^="#"]')) {
        e.preventDefault();
        const target = document.querySelector(e.target.getAttribute('href'));
        if (target) {
          const headerHeight = document.querySelector('.header')?.offsetHeight || 0;
          const targetPosition = target.offsetTop - headerHeight - 20;
          
          window.scrollTo({
            top: targetPosition,
            behavior: 'smooth'
          });
        }
      }
    });
  }

  /**
   * Progress Tracking
   */
  setupProgressTracking() {
    // Track reading progress
    const progressBar = document.createElement('div');
    progressBar.className = 'reading-progress';
    progressBar.style.cssText = `
      position: fixed;
      top: 0;
      left: 0;
      width: 0%;
      height: 3px;
      background: linear-gradient(90deg, var(--mtm-primary), var(--mtm-magenta));
      z-index: 1000;
      transition: width 0.2s ease;
    `;
    document.body.appendChild(progressBar);

    window.addEventListener('scroll', () => {
      const windowHeight = window.innerHeight;
      const documentHeight = document.documentElement.scrollHeight - windowHeight;
      const scrollTop = window.pageYOffset;
      const progress = (scrollTop / documentHeight) * 100;
      
      progressBar.style.width = Math.min(progress, 100) + '%';
    });

    // Track task completion if there are checkboxes
    this.trackTaskCompletion();
  }

  trackTaskCompletion() {
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');
    if (checkboxes.length === 0) return;

    const updateProgress = () => {
      const total = checkboxes.length;
      const completed = Array.from(checkboxes).filter(cb => cb.checked).length;
      const percentage = Math.round((completed / total) * 100);
      
      // Update any progress indicators
      const progressIndicators = document.querySelectorAll('.task-progress');
      progressIndicators.forEach(indicator => {
        indicator.textContent = `${completed}/${total} tasks completed (${percentage}%)`;
      });
    };

    checkboxes.forEach(checkbox => {
      checkbox.addEventListener('change', updateProgress);
    });

    updateProgress();
  }

  /**
   * Tooltips
   */
  setupTooltips() {
    const tooltipElements = document.querySelectorAll('[data-tooltip]');
    
    tooltipElements.forEach(element => {
      element.addEventListener('mouseenter', (e) => {
        this.showTooltip(e.target);
      });
      
      element.addEventListener('mouseleave', (e) => {
        this.hideTooltip(e.target);
      });
    });
  }

  showTooltip(element) {
    const tooltip = element.getAttribute('data-tooltip');
    if (!tooltip) return;

    const tooltipElement = document.createElement('div');
    tooltipElement.className = 'tooltip-popup';
    tooltipElement.textContent = tooltip;
    tooltipElement.style.cssText = `
      position: absolute;
      background: var(--color-gray-900);
      color: white;
      padding: 0.5rem 0.75rem;
      border-radius: var(--radius-md);
      font-size: var(--font-size-sm);
      white-space: nowrap;
      z-index: 1000;
      pointer-events: none;
      opacity: 0;
      transition: opacity 0.2s ease;
    `;

    document.body.appendChild(tooltipElement);

    const rect = element.getBoundingClientRect();
    tooltipElement.style.left = rect.left + (rect.width / 2) - (tooltipElement.offsetWidth / 2) + 'px';
    tooltipElement.style.top = rect.top - tooltipElement.offsetHeight - 8 + 'px';
    
    setTimeout(() => tooltipElement.style.opacity = '1', 10);
    
    element._tooltip = tooltipElement;
  }

  hideTooltip(element) {
    if (element._tooltip) {
      element._tooltip.remove();
      delete element._tooltip;
    }
  }

  /**
   * Keyboard Navigation
   */
  setupKeyboardNavigation() {
    document.addEventListener('keydown', (e) => {
      // ESC key closes search and mobile menu
      if (e.key === 'Escape') {
        this.closeMobileMenu();
        const searchResults = document.querySelector('.search-results');
        if (searchResults) {
          searchResults.style.display = 'none';
        }
      }

      // Ctrl/Cmd + K opens search
      if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        const searchInput = document.querySelector('.search-input');
        if (searchInput) {
          searchInput.focus();
        }
      }

      // Arrow keys for navigation
      if (e.target.matches('.nav-link, .mtm-nav-link')) {
        if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
          e.preventDefault();
          const links = Array.from(document.querySelectorAll('.nav-link, .mtm-nav-link'));
          const currentIndex = links.indexOf(e.target);
          const nextIndex = e.key === 'ArrowDown' 
            ? Math.min(currentIndex + 1, links.length - 1)
            : Math.max(currentIndex - 1, 0);
          links[nextIndex]?.focus();
        }
      }
    });
  }

  /**
   * Accessibility Enhancements
   */
  setupAccessibility() {
    // Add skip link if it doesn't exist
    if (!document.querySelector('.skip-to-content')) {
      const skipLink = document.createElement('a');
      skipLink.href = '#main-content';
      skipLink.className = 'skip-to-content';
      skipLink.textContent = 'Skip to main content';
      document.body.insertBefore(skipLink, document.body.firstChild);
    }

    // Add main content landmark
    const main = document.querySelector('main, .main');
    if (main && !main.id) {
      main.id = 'main-content';
    }

    // Enhance details/summary accessibility
    const summaryElements = document.querySelectorAll('summary');
    summaryElements.forEach(summary => {
      if (!summary.getAttribute('aria-expanded')) {
        const details = summary.closest('details');
        summary.setAttribute('aria-expanded', details?.open ? 'true' : 'false');
        
        details?.addEventListener('toggle', () => {
          summary.setAttribute('aria-expanded', details.open ? 'true' : 'false');
        });
      }
    });

    // Add proper headings hierarchy
    this.validateHeadingHierarchy();
  }

  validateHeadingHierarchy() {
    const headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
    let previousLevel = 0;
    
    headings.forEach(heading => {
      const level = parseInt(heading.tagName.charAt(1));
      
      if (level > previousLevel + 1) {
        console.warn(`Heading hierarchy skip detected: ${heading.tagName} after h${previousLevel}`, heading);
      }
      
      previousLevel = level;
    });
  }

  /**
   * Element Animations
   */
  animateElements() {
    // Intersection Observer for fade-in animations
    const observerOptions = {
      threshold: 0.1,
      rootMargin: '0px 0px -10% 0px'
    };

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('mtm-fade-in');
          observer.unobserve(entry.target);
        }
      });
    }, observerOptions);

    // Observe cards and main content sections
    const animatedElements = document.querySelectorAll('.card, .mtm-card, .alert, section');
    animatedElements.forEach(element => {
      observer.observe(element);
    });

    // Add stagger animation to navigation links
    const navLinks = document.querySelectorAll('.nav-link, .mtm-nav-link');
    navLinks.forEach((link, index) => {
      link.style.animationDelay = `${index * 0.1}s`;
      link.classList.add('mtm-slide-up');
    });
  }

  /**
   * Utility Methods
   */
  debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
      const later = () => {
        clearTimeout(timeout);
        func(...args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  }

  throttle(func, limit) {
    let inThrottle;
    return function(...args) {
      if (!inThrottle) {
        func.apply(this, args);
        inThrottle = true;
        setTimeout(() => inThrottle = false, limit);
      }
    };
  }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
  new MTMDocumentation();
});

// Export for potential module usage
if (typeof module !== 'undefined' && module.exports) {
  module.exports = MTMDocumentation;
}