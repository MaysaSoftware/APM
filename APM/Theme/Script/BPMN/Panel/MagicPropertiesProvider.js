
const magicPropertiesProviderModule = {
    __init__: ['magicPropertiesProvider'],
        magicPropertiesProvider: ['type', MagicPropertiesProvider]
};
 

const LOW_PRIORITY = 500;
 
  function  MagicPropertiesProvider(propertiesPanel, translate) {

    // API ////////

    /**
     * Return the groups provided for the given element.
     *
     * @param {DiagramElement} element
     *
     * @return {(Object[]) => (Object[])} groups middleware
     */
    this.getGroups = function (element) {

        /**
         * We return a middleware that modifies
         * the existing groups.
         *
         * @param {Object[]} groups
         *
         * @return {Object[]} modified groups
         */
        return function (groups) {

            // Add the "magic" group
            //if (is(element, 'bpmn:StartEvent')) {
            //    groups.push(createMagicGroup(element, translate));
            //}

            return groups;
        };
    };


    // registration ////////

    // Register our custom magic properties provider.
    // Use a lower priority to ensure it is loaded after
    // the basic BPMN properties.
    propertiesPanel.registerProvider(LOW_PRIORITY, this);
}

MagicPropertiesProvider.$inject = ['propertiesPanel', 'translate'];

// Create the custom magic group
function createMagicGroup(element, translate) {

    // create a group called "Magic properties".
    const magicGroup = {
        id: 'magic',
        label: translate('Magic properties'),
        entries: spellProps(element),
        tooltip: translate('Make sure you know what you are doing!')
    };

    return magicGroup;
}
