export function parseApiDate(apiDate) {
    //2021-10-10T14:25:13.063Z
    let year = '', month = '', day = '', hour = '', min = '', sec = ''
    if (apiDate && apiDate != 'null' && apiDate.length > 20) {
        year = parseInt(apiDate.substr(0, 4))
        month = parseInt(apiDate.substr(5, 2))
        day = parseInt(apiDate.substr(8, 2))
        hour = parseInt(apiDate.substr(11, 2))
        min = parseInt(apiDate.substr(14, 2))
        sec = parseInt(apiDate.substr(17, 2))
    }
    return {
        year, month, day, hour, min, sec
    }
}

export function relativeDate(apiDate) {
    if (apiDate && apiDate != 'null' && apiDate.length > 20) {

        let date = parseApiDate(apiDate)

        let returnHour = `${pad(date.hour,2)}:${pad(date.min,2)}:${pad(date.sec,2)}`

        let now = new Date()

        var year = now.getFullYear();
        var month = now.getMonth() + 1;
        var day = now.getDate();

        if (date.year != year) {
            return `${pad(date.year,2)}-${pad(date.month,2)}-${pad(date.day,2)}` + returnHour
        }

        if (date.month != month || date.day != day) {
            return `${pad(date.month,2)}-${pad(date.day,2)}` + returnHour
        }
        return returnHour
    }
    return "-"
}

function pad(val,digitCount) {
    return val.toString().padStart(digitCount,'0')
}