 

const HANDLERS = {
  'properties-panel.multi-command-executor': MultiCommandHandler
};


function CommandInitializer(eventBus, commandStack) {

  eventBus.on('diagram.init', function() {
    forEach(HANDLERS, function(handler, id) {
      commandStack.registerHandler(id, handler);
    });
  });
}

CommandInitializer.$inject = [ 'eventBus', 'commandStack' ];

const Commands= {
  __init__: [ CommandInitializer ]
};