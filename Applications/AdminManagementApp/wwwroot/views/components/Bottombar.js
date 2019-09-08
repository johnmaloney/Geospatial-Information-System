let Bottombar = {
    render: async () => {
        let view =  /*html*/`
        <footer class="footer">
            <div class="content has-text-centered">
                <p>
                    GIS Administration ${Date(Date.now()).toString()}
                </p>
            </div>
        </footer>
        `;
        return view;
    },
    after_render: async () => { }

}

export default Bottombar;