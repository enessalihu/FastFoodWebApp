// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Add active class to current nav item
    const currentLocation = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link');
    
    navLinks.forEach(link => {
        const linkPath = link.getAttribute('href');
        if (currentLocation === linkPath || 
            (linkPath !== '/' && currentLocation.startsWith(linkPath))) {
            link.classList.add('active');
        }
    });

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Add smooth scrolling to all links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth'
                });
            }
        });
    });

    // Add animation to cards on scroll
    const animateOnScroll = () => {
        const elements = document.querySelectorAll('.card, .section-title, .about-image, .team-member');
        elements.forEach(element => {
            const elementPosition = element.getBoundingClientRect().top;
            const screenPosition = window.innerHeight / 1.2;
            
            if (elementPosition < screenPosition) {
                element.style.opacity = '1';
                element.style.transform = 'translateY(0)';
            }
        });
    };

    // Set initial styles for animation
    const elementsToAnimate = document.querySelectorAll('.card, .section-title, .about-image, .team-member');
    elementsToAnimate.forEach(element => {
        element.style.opacity = '0';
        element.style.transform = 'translateY(20px)';
        element.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
    });

    // Call the animation function on scroll
    window.addEventListener('scroll', animateOnScroll);
    
    // Call it once on load
    setTimeout(animateOnScroll, 100);

    // Handle quantity input in product details
    const quantityInput = document.getElementById('quantity');
    if (quantityInput) {
        quantityInput.addEventListener('change', function() {
            if (this.value < 1) {
                this.value = 1;
            } else if (this.value > 10) {
                this.value = 10;
            }
        });
    }

    // Handle payment method toggle in checkout
    const paymentMethodRadios = document.querySelectorAll('input[name="PaymentMethod"]');
    const cardPaymentDetails = document.getElementById('cardPaymentDetails');
    
    if (paymentMethodRadios.length > 0 && cardPaymentDetails) {
        paymentMethodRadios.forEach(radio => {
            radio.addEventListener('change', function() {
                if (this.value === 'cash') {
                    cardPaymentDetails.style.display = 'none';
                } else {
                    cardPaymentDetails.style.display = 'block';
                }
            });
        });
    }

    // Sticky header on scroll
    const header = document.querySelector('header');
    if (header) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 50) {
                header.classList.add('sticky-top');
                header.style.backgroundColor = 'rgba(255, 255, 255, 0.95)';
                header.style.backdropFilter = 'blur(10px)';
            } else {
                header.classList.remove('sticky-top');
                header.style.backgroundColor = 'white';
                header.style.backdropFilter = 'none';
            }
        });
    }
});