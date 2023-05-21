e => {
    const rect = e.getBoundingClientRect();
    
    return { 
        x: rect.x, 
        y: rect.y, 
        width: rect.width, 
        height: rect.height
    };
}