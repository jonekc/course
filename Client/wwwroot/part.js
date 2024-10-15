function fixScrolling() {
    let i = 0;
    const timer = setInterval(() => {
        if (i < 20) {
            const pickers = document.querySelectorAll(".ql-picker");
            if (pickers) {
                pickers.forEach(tool => {
                    tool.addEventListener("mousedown", (event) => {
                        event.preventDefault();
                        event.stopPropagation();
                    });
                })
            }
            i++;
        }
        else {
            clearInterval(timer);
        }
    }, 500)
}