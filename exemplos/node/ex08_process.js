

function temPram(param) {
    return process.argv.indexOf(param) !== -1
}


if (temPram('--producao')) {
    console.log('Atenção!!!')
} else {
    console.log('Tranquilo!!!')
}