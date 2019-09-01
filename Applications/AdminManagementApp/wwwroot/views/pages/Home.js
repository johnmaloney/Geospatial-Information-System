// --------------------------------
//  Define Data Sources
// --------------------------------

let getMessageList = async () => {
    const options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    };
    try {
        const response = await fetch(`api/message`, options);
        const json = await response.json();
        // console.log(json)
        return json;
    } catch (err) {
        console.log('Error getting documents', err);
    }
};

let Home = {
    render: async () => {
        let messages = await getMessageList();
        let view =  /*html*/`
            <section class="section">
                <h1> Home </h1>
                <ul>
                    ${ messages.map(m =>
                        /*html*/`<li><a href="#/p/${m.id}">${m.type}</a></li>`
        ).join('\n ')
            }
                </ul>
            </section>
        `;
        return view;
    }
    , after_render: async () => {
    }

};

export default Home;