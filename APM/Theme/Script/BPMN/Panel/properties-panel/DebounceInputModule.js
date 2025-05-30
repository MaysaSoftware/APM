
const DEFAULT_DEBOUNCE_TIME = 300;

function debounceInput(debounceDelay) {
    return function _debounceInput(fn) {
        if (debounceDelay !== false) {

            var debounceTime =
                isNumber(debounceDelay) ?
                    debounceDelay :
                    DEFAULT_DEBOUNCE_TIME;

            return debounce(fn, debounceTime);
        } else {
            return fn;
        }
    };
}

debounceInput.$inject = ['config.debounceInput'];

const DebounceInputModule = {
    debounceInput: ['factory', debounceInput]
};

