 
const BpmnPropertiesPanelModule= {
  __depends__: [
    Commands,
    DebounceInputModule,
    _FeelPopupModule
  ],
  __init__: [
    'propertiesPanel'
  ],
  propertiesPanel: [ 'type', BpmnPropertiesPanelRenderer ]
};