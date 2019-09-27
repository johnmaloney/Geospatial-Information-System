"use strict";

// Credit to https://github.com/rishavs/vanillajs-spa

import Home from './views/pages/Home.js';
import About from './views/pages/About.js';
import JobEntry from './views/components/JobEntry.js';
import LogEntry from './views/components/LogEntry.js';
import Utility from './views/components/Utility.js';
import Map from './views/components/Map.js';
import Messages from './views/components/Messages.js';
import Layer from './views/components/Layer.js';
import Error404 from './views/pages/Error404.js';
import PostShow from './views/pages/PostShow.js';
import Register from './views/pages/Register.js';

import Navbar from './views/components/Navbar.js';
import Bottombar from './views/components/Bottombar.js';

import Utils from './services/Utils.js';

// List of supported routes. Any url other than these routes will throw a 404 error
const routes = {
    '/': Home
    , '/about': About
    , '/p/:id': PostShow
    , '/register': Register
};

let SessionIdentifier = "Unknown";

// The router code. Takes a URL, checks against the list of supported routes and then renders the corresponding content page.
const router = async () => {

    // Lazy load view element:
    const header = null || document.getElementById('header_container');
    //const content = null || document.getElementById('page_container');
    const utilityItems = null || document.getElementById('utility_container');
    const jobEntryForm = null || document.getElementById('jobentry_container');
    const layerSets = null || document.getElementById('layer_container');

    const map = null || document.getElementById('map_container');
    const messages = null || document.getElementById('message_container');  
    
    const footer = null || document.getElementById('footer_container');
    
    // Render the Header and footer of the page
    header.innerHTML = await Navbar.render();
    await Navbar.after_render();
    footer.innerHTML = await Bottombar.render();
    await Bottombar.after_render();
    
    // Get the parsed URl from the addressbar
    let request = Utils.parseRequestURL();

    // Parse the URL and if it has an id part, change it with the string ":id"
    let parsedURL = (request.resource ? '/' + request.resource : '/') + (request.id ? '/:id' : '') + (request.verb ? '/' + request.verb : '')
    
    // Get the page from our hash of supported routes.
    // If the parsed URL is not in our list of supported routes, select the 404 page instead
    //let page = routes[parsedURL] ? routes[parsedURL] : Error404;
    //content.innerHTML = await page.render();

    utilityItems.innerHTML = await Utility.render();
    await Utility.after_render();

    map.innerHTML = await Map.render();
    await Map.after_render();

    messages.innerHTML = await Messages.render();
    await Messages.after_render();

    jobEntryForm.innerHTML = await JobEntry.render();
    await JobEntry.after_render();

    layerSets.innerHTML = await Layer.render();
    await Layer.after_render();
    

    //await page.after_render();
}

// Listen on hash change:
window.addEventListener('hashchange', router);

// Listen on page load:
window.addEventListener('load', router);

L.Map.addInitHook(function () {
    mapsPlaceholder.push(this); // Use whatever global scope variable you like.
});

