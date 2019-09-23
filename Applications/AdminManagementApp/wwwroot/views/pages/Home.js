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
            <section class="section" style="height:50px;overflow-y:scroll">
                <h1> Home </h1>
                <ul>
                    ${ messages.map(m =>
                        /*html*/`<li><a href="#/p/${m.id}">${m.message}</a></li>`
                    ).join('\n ')
                }
                </ul>
            </section>
            <section class="section">
                <button class="button is-link" id="refreshMessages">Refresh</button>
            </section>
        `;
        return view;
    }
    , after_render: async () => {
        var form = document.getElementById("refreshMessages");
        form.addEventListener("click", function (event) {
            //event.preventDefault();
            Home.render().then(function (view) {

                document.getElementById('page_container').innerHTML = view;
            });
        });
    }

};

export default Home;