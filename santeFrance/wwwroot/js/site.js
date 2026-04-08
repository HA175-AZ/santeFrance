// Site JavaScript pour SantéFrance

// Fonction pour animer les éléments au scroll
function animateOnScroll() {
    const elements = document.querySelectorAll('.help-card, .resource-card, .info-card, .emergency-card-main');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, {
        threshold: 0.1
    });

    elements.forEach(element => {
        element.style.opacity = '0';
        element.style.transform = 'translateY(20px)';
        element.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(element);
    });
}

// Fonction pour smooth scroll
function smoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// Menu mobile toggle
function initMobileMenu() {
    const menuToggle = document.querySelector('.menu-toggle');
    const navMenu = document.querySelector('.navbar-menu');

    if (menuToggle && navMenu) {
        menuToggle.addEventListener('click', () => {
            menuToggle.classList.toggle('active');
            navMenu.classList.toggle('active');
        });

        // Fermer le menu quand on clique sur un lien
        navMenu.querySelectorAll('.nav-links a').forEach(link => {
            link.addEventListener('click', () => {
                menuToggle.classList.remove('active');
                navMenu.classList.remove('active');
            });
        });

        // Fermer le menu quand on clique en dehors
        document.addEventListener('click', (e) => {
            if (!menuToggle.contains(e.target) && !navMenu.contains(e.target)) {
                menuToggle.classList.remove('active');
                navMenu.classList.remove('active');
            }
        });
    }
}

// Sélecteur de langue
function initLanguageSelector() {
    const selector = document.getElementById('langSelector');
    const dropdown = document.getElementById('langDropdown');
    const currentLang = document.getElementById('langCurrent');
    const arrow = document.getElementById('langArrow');

    if (!selector || !dropdown) return;

    // Ouvrir/fermer le dropdown
    selector.addEventListener('click', (e) => {
        e.stopPropagation();
        selector.classList.toggle('open');
    });

    // Sélectionner une langue
    dropdown.querySelectorAll('.lang-option').forEach(option => {
        option.addEventListener('click', (e) => {
            e.stopPropagation();

            // Retirer la classe active de toutes les options
            dropdown.querySelectorAll('.lang-option').forEach(opt => opt.classList.remove('active'));

            // Ajouter la classe active à l'option sélectionnée
            option.classList.add('active');

            // Mettre à jour le texte affiché
            const langNames = {
                'fr': 'Français',
                'en': 'English',
                'es': 'Español',
                'ar': 'العربية',
                'zh': '中文'
            };
            const lang = option.getAttribute('data-lang');
            currentLang.textContent = langNames[lang] || lang;

            // Fermer le dropdown
            selector.classList.remove('open');

            // Sauvegarder la préférence de langue
            localStorage.setItem('santefrance-lang', lang);
        });
    });

    // Fermer quand on clique en dehors
    document.addEventListener('click', () => {
        selector.classList.remove('open');
    });

    // Restaurer la langue sauvegardée
    const savedLang = localStorage.getItem('santefrance-lang');
    if (savedLang) {
        const savedOption = dropdown.querySelector(`[data-lang="${savedLang}"]`);
        if (savedOption) {
            dropdown.querySelectorAll('.lang-option').forEach(opt => opt.classList.remove('active'));
            savedOption.classList.add('active');
            const langNames = {
                'fr': 'Français',
                'en': 'English',
                'es': 'Español',
                'ar': 'العربية',
                'zh': '中文'
            };
            currentLang.textContent = langNames[savedLang] || savedLang;
        }
    }
}

// Effet de parallaxe simple sur le hero
function initParallax() {
    const hero = document.querySelector('.hero-image');

    if (hero) {
        window.addEventListener('scroll', () => {
            const scrolled = window.pageYOffset;
            hero.style.transform = `translateY(${scrolled * 0.3}px)`;
        });
    }
}

// Animation pour les numéros d'urgence
function animateEmergencyNumbers() {
    const emergencyCards = document.querySelectorAll('.emergency-card-main');

    emergencyCards.forEach((card, index) => {
        setTimeout(() => {
            card.style.opacity = '0';
            card.style.transform = 'scale(0.8)';
            card.style.transition = 'all 0.5s ease';

            setTimeout(() => {
                card.style.opacity = '1';
                card.style.transform = 'scale(1)';
            }, 100);
        }, index * 100);
    });
}

// Fonction pour gérer le clic sur les numéros d'urgence
function handleEmergencyCalls() {
    const emergencyCards = document.querySelectorAll('.emergency-card-main');

    emergencyCards.forEach(card => {
        card.addEventListener('click', function () {
            const number = this.querySelector('.emergency-number').textContent;
            const title = this.querySelector('.emergency-title').textContent;

            // Animation de confirmation
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = 'scale(1)';
            }, 200);

            // Confirmation avant d'appeler (optionnel)
            if (confirm(`Voulez-vous appeler le ${number} (${title}) ?\n\nCette action ouvrira votre application téléphone.`)) {
                window.location.href = `tel:${number}`;
            }
        });

        // Curseur pointer sur les cartes
        card.style.cursor = 'pointer';
    });
}

// Initialisation au chargement de la page
document.addEventListener('DOMContentLoaded', () => {
    animateOnScroll();
    smoothScroll();
    initMobileMenu();
    initLanguageSelector();
    initParallax();

    // Animations spécifiques à la page Urgences
    if (document.querySelector('.emergency-card-main')) {
        animateEmergencyNumbers();
        handleEmergencyCalls();
    }

    // Animation du logo au chargement
    const logo = document.querySelector('.logo-icon');
    if (logo) {
        logo.style.animation = 'pulse 2s ease-in-out infinite';
    }

    // Animation de l'alerte urgence
    const alertBox = document.querySelector('.alert-box');
    if (alertBox) {
        alertBox.style.animation = 'pulseAlert 2s ease-in-out infinite';
    }
});

// Style pour l'animation pulse (ajouté dynamiquement)
const style = document.createElement('style');
style.textContent = `
    @keyframes pulse {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.05); }
    }
    
    @keyframes pulseAlert {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.02); }
    }
`;
document.head.appendChild(style);

// Fonction pour afficher un message de confirmation
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 16px 24px;
        background: ${type === 'success' ? '#0D9488' : '#F43F5E'};
        color: white;
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        z-index: 9999;
        animation: slideIn 0.3s ease;
        font-weight: 600;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Animations CSS pour les notifications
const animationStyle = document.createElement('style');
animationStyle.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(animationStyle);