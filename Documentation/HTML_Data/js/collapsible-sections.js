// MTM Documentation Collapsible Sections
document.addEventListener('DOMContentLoaded', function() {
    initializeCollapsibleSections();
    initializeDetailsElements();
});

function initializeCollapsibleSections() {
    // Handle custom collapsible elements
    const collapsibles = document.querySelectorAll('.collapsible');
    
    collapsibles.forEach(collapsible => {
        const header = collapsible.querySelector('.collapsible-header');
        const content = collapsible.querySelector('.collapsible-content');
        
        if (header && content) {
            header.addEventListener('click', function() {
                toggleCollapsible(collapsible);
            });
            
            // Add keyboard support
            header.setAttribute('tabindex', '0');
            header.addEventListener('keydown', function(e) {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    toggleCollapsible(collapsible);
                }
            });
        }
    });
}

function initializeDetailsElements() {
    // Handle HTML5 details/summary elements with enhanced styling
    const details = document.querySelectorAll('details');
    
    details.forEach(detail => {
        const summary = detail.querySelector('summary');
        
        if (summary) {
            // Add enhanced styling classes
            summary.classList.add('mtm-summary');
            detail.classList.add('mtm-details');
            
            // Add custom toggle behavior
            summary.addEventListener('click', function(e) {
                // Let the default behavior happen, but add our animations
                setTimeout(() => {
                    updateDetailsState(detail);
                }, 10);
            });
            
            // Initialize state
            updateDetailsState(detail);
        }
    });
}

function toggleCollapsible(collapsible) {
    const isActive = collapsible.classList.contains('active');
    const content = collapsible.querySelector('.collapsible-content');
    
    if (isActive) {
        // Closing
        collapsible.classList.remove('active');
        slideUp(content);
    } else {
        // Opening
        collapsible.classList.add('active');
        slideDown(content);
    }
    
    // Fire custom event
    const event = new CustomEvent('collapsibleToggle', {
        detail: {
            element: collapsible,
            isOpen: !isActive
        }
    });
    document.dispatchEvent(event);
}

function updateDetailsState(detail) {
    const summary = detail.querySelector('summary');
    
    if (detail.open) {
        detail.classList.add('open');
        summary.classList.add('open');
    } else {
        detail.classList.remove('open');
        summary.classList.remove('open');
    }
}

function slideDown(element) {
    element.style.display = 'block';
    element.style.height = '0px';
    element.style.overflow = 'hidden';
    element.style.transition = 'height 0.3s ease-out';
    
    // Get the natural height
    const height = element.scrollHeight + 'px';
    
    // Trigger the transition
    requestAnimationFrame(() => {
        element.style.height = height;
    });
    
    // Clean up after animation
    setTimeout(() => {
        element.style.height = '';
        element.style.overflow = '';
        element.style.transition = '';
    }, 300);
}

function slideUp(element) {
    element.style.height = element.scrollHeight + 'px';
    element.style.overflow = 'hidden';
    element.style.transition = 'height 0.3s ease-out';
    
    // Trigger the transition
    requestAnimationFrame(() => {
        element.style.height = '0px';
    });
    
    // Hide after animation
    setTimeout(() => {
        element.style.display = 'none';
        element.style.height = '';
        element.style.overflow = '';
        element.style.transition = '';
    }, 300);
}

// Expand all / Collapse all functionality
function expandAllSections() {
    const collapsibles = document.querySelectorAll('.collapsible:not(.active)');
    const details = document.querySelectorAll('details:not([open])');
    
    collapsibles.forEach(collapsible => {
        toggleCollapsible(collapsible);
    });
    
    details.forEach(detail => {
        detail.open = true;
        updateDetailsState(detail);
    });
}

function collapseAllSections() {
    const collapsibles = document.querySelectorAll('.collapsible.active');
    const details = document.querySelectorAll('details[open]');
    
    collapsibles.forEach(collapsible => {
        toggleCollapsible(collapsible);
    });
    
    details.forEach(detail => {
        detail.open = false;
        updateDetailsState(detail);
    });
}

// Add expand/collapse all controls if they exist
function initializeGlobalControls() {
    const expandAllBtn = document.querySelector('.expand-all');
    const collapseAllBtn = document.querySelector('.collapse-all');
    
    if (expandAllBtn) {
        expandAllBtn.addEventListener('click', expandAllSections);
    }
    
    if (collapseAllBtn) {
        collapseAllBtn.addEventListener('click', collapseAllSections);
    }
}

// Initialize global controls when DOM is ready
document.addEventListener('DOMContentLoaded', initializeGlobalControls);

// Auto-collapse sections based on screen size
function handleResponsiveCollapse() {
    const isMobile = window.innerWidth <= 768;
    const autoCollapseElements = document.querySelectorAll('.auto-collapse-mobile');
    
    autoCollapseElements.forEach(element => {
        if (isMobile && element.classList.contains('active')) {
            toggleCollapsible(element);
        } else if (!isMobile && !element.classList.contains('active')) {
            toggleCollapsible(element);
        }
    });
}

// Handle window resize
window.addEventListener('resize', function() {
    clearTimeout(window.resizeTimeout);
    window.resizeTimeout = setTimeout(handleResponsiveCollapse, 250);
});

// Initial responsive check
document.addEventListener('DOMContentLoaded', handleResponsiveCollapse);

// Save and restore section states in localStorage
function saveCollapsibleStates() {
    const states = {};
    const collapsibles = document.querySelectorAll('.collapsible[id]');
    
    collapsibles.forEach(collapsible => {
        states[collapsible.id] = collapsible.classList.contains('active');
    });
    
    const details = document.querySelectorAll('details[id]');
    details.forEach(detail => {
        states[detail.id] = detail.open;
    });
    
    localStorage.setItem('mtm-doc-collapsible-states', JSON.stringify(states));
}

function restoreCollapsibleStates() {
    const statesJson = localStorage.getItem('mtm-doc-collapsible-states');
    if (!statesJson) return;
    
    try {
        const states = JSON.parse(statesJson);
        
        Object.keys(states).forEach(id => {
            const element = document.getElementById(id);
            if (!element) return;
            
            if (element.classList.contains('collapsible')) {
                const shouldBeActive = states[id];
                const isActive = element.classList.contains('active');
                
                if (shouldBeActive !== isActive) {
                    toggleCollapsible(element);
                }
            } else if (element.tagName.toLowerCase() === 'details') {
                element.open = states[id];
                updateDetailsState(element);
            }
        });
    } catch (e) {
        console.warn('Failed to restore collapsible states:', e);
    }
}

// Save states on page unload
window.addEventListener('beforeunload', saveCollapsibleStates);

// Restore states on page load
document.addEventListener('DOMContentLoaded', function() {
    // Delay restoration to ensure all elements are initialized
    setTimeout(restoreCollapsibleStates, 100);
});

// Add CSS classes for enhanced styling
document.addEventListener('DOMContentLoaded', function() {
    // Add MTM styling to existing elements
    const summaries = document.querySelectorAll('summary');
    summaries.forEach(summary => {
        if (!summary.classList.contains('mtm-summary')) {
            summary.classList.add('mtm-summary');
        }
    });
    
    const details = document.querySelectorAll('details');
    details.forEach(detail => {
        if (!detail.classList.contains('mtm-details')) {
            detail.classList.add('mtm-details');
        }
    });
});

// Export functions for external use
window.MTMCollapsible = {
    toggleCollapsible,
    expandAllSections,
    collapseAllSections,
    saveCollapsibleStates,
    restoreCollapsibleStates
};