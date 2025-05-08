/**
 * Form Extras
 */

'use strict';

(function () {
  const prefixMask = document.querySelector('.prefix-mask');


  // Prefix
  if (prefixMask) {
    new Cleave(prefixMask, {
      prefix: 'TSM',
      blocks: [8],
      uppercase: true,
      numericOnly: true 
    });
  }
})();


