////import {
////  has,
////  isNil,
////  isObject
////} from 'min-dash';

/**
 * Checks whether a value is an instance of Connection.
 *
 * @param {any} value
 *
 * @return {boolean}
 */
function isConnection(value) {
    return isObject(value) && has(value, 'waypoints');
}

/**
 * Checks whether a value is an instance of Label.
 *
 * @param {any} value
 *
 * @return {boolean}
 */
function isLabel(value) {
    return isObject(value) && has(value, 'labelTarget');
}

/**
 * Checks whether a value is an instance of Root.
 *
 * @param {any} value
 *
 * @return {boolean}
 */
function isRoot(value) {
    return isObject(value) && isNil(value.parent);
}


////import {
////  some
////} from 'min-dash';

/**
 * @typedef { import('../model/Types').Element } Element
 * @typedef { import('../model/Types').ModdleElement } ModdleElement
 */

/**
 * Is an element of the given BPMN type?
 *
 * @param  {Element|ModdleElement} element
 * @param  {string} type
 *
 * @return {boolean}
 */
function is(element, type) {
    var bo = getBusinessObject(element);

    return bo && (typeof bo.$instanceOf === 'function') && bo.$instanceOf(type);
}


/**
 * Return true if element has any of the given types.
 *
 * @param {Element|ModdleElement} element
 * @param {string[]} types
 *
 * @return {boolean}
 */
function isAny(element, types) {
    return some(types, function (t) {
        return is(element, t);
    });
}

/**
 * Return the business object for a given element.
 *
 * @param {Element|ModdleElement} element
 *
 * @return {ModdleElement}
 */
function getBusinessObject(element) {
    return (element && element.businessObject) || element;
}

/**
 * Return the di object for a given element.
 *
 * @param {Element} element
 *
 * @return {ModdleElement}
 */
function getDi(element) {
    return element && element.di;
}