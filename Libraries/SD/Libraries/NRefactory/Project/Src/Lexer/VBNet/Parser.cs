using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 55;
	const int startOfBlock = 261;
	const int endOfStatementTerminatorAndBlock = 265;
	static BitArray GetExpectedSet(int state)
	{
		switch (state) {
			case 0:
			case 1:
				return set[0];
			case 2:
				return set[1];
			case 3:
			case 4:
				return set[2];
			case 5:
				return set[3];
			case 6:
			case 88:
			case 266:
			case 534:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 10:
			case 20:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 11:
			case 193:
			case 199:
			case 205:
			case 244:
			case 248:
			case 297:
			case 401:
			case 407:
			case 478:
			case 524:
			case 531:
			case 539:
			case 569:
			case 605:
			case 654:
			case 668:
			case 741:
				return set[6];
			case 12:
			case 13:
			case 570:
			case 571:
			case 616:
			case 626:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 14:
			case 21:
			case 23:
			case 24:
			case 36:
			case 258:
			case 261:
			case 262:
			case 298:
			case 302:
			case 324:
			case 339:
			case 350:
			case 353:
			case 359:
			case 364:
			case 374:
			case 375:
			case 398:
			case 425:
			case 530:
			case 536:
			case 542:
			case 546:
			case 554:
			case 562:
			case 572:
			case 581:
			case 598:
			case 603:
			case 611:
			case 617:
			case 620:
			case 627:
			case 630:
			case 649:
			case 652:
			case 676:
			case 684:
			case 720:
			case 740:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 15:
			case 16:
				return set[7];
			case 17:
			case 18:
				return set[8];
			case 19:
			case 259:
			case 273:
			case 300:
			case 354:
			case 399:
			case 458:
			case 579:
			case 599:
			case 618:
			case 622:
			case 628:
			case 650:
			case 685:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 547:
			case 582:
				return set[9];
			case 25:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 26:
			case 27:
				return set[10];
			case 28:
			case 724:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 152:
			case 217:
			case 218:
			case 267:
			case 278:
			case 279:
			case 466:
			case 467:
			case 468:
			case 469:
			case 557:
			case 558:
			case 591:
			case 592:
			case 679:
			case 680:
			case 733:
			case 734:
				return set[14];
			case 33:
			case 34:
			case 525:
			case 526:
			case 532:
			case 533:
			case 559:
			case 560:
			case 673:
				return set[15];
			case 35:
			case 37:
			case 157:
			case 168:
			case 171:
			case 187:
			case 203:
			case 221:
			case 309:
			case 334:
			case 424:
			case 442:
			case 453:
			case 481:
			case 535:
			case 553:
			case 561:
			case 633:
			case 636:
			case 658:
			case 661:
			case 663:
			case 675:
			case 688:
			case 690:
			case 713:
			case 716:
			case 719:
			case 725:
			case 728:
			case 746:
				return set[16];
			case 38:
			case 41:
				return set[17];
			case 39:
				return set[18];
			case 40:
			case 97:
			case 101:
			case 163:
			case 390:
			case 485:
				return set[19];
			case 42:
				{
					BitArray a = new BitArray(239);
					a.Set(33, true);
					a.Set(37, true);
					return a;
				}
			case 43:
			case 44:
			case 165:
			case 166:
				return set[20];
			case 45:
			case 46:
			case 167:
			case 188:
			case 394:
			case 429:
			case 479:
			case 482:
			case 502:
			case 565:
			case 596:
			case 648:
			case 694:
			case 723:
			case 732:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 47:
			case 48:
				return set[21];
			case 49:
			case 179:
			case 186:
			case 396:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 50:
			case 51:
			case 52:
			case 54:
			case 392:
			case 393:
			case 414:
			case 415:
			case 421:
			case 422:
			case 493:
			case 494:
			case 707:
			case 708:
				return set[22];
			case 53:
			case 169:
			case 170:
			case 172:
			case 181:
			case 416:
			case 423:
			case 431:
			case 440:
			case 444:
			case 489:
			case 492:
			case 496:
			case 498:
			case 499:
			case 509:
			case 516:
			case 523:
			case 709:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 55:
			case 56:
			case 70:
			case 75:
			case 76:
			case 77:
			case 83:
			case 99:
			case 155:
			case 178:
			case 180:
			case 182:
			case 185:
			case 195:
			case 197:
			case 215:
			case 239:
			case 276:
			case 286:
			case 288:
			case 289:
			case 306:
			case 323:
			case 328:
			case 337:
			case 343:
			case 345:
			case 349:
			case 352:
			case 358:
			case 369:
			case 371:
			case 372:
			case 378:
			case 395:
			case 397:
			case 417:
			case 441:
			case 471:
			case 487:
			case 488:
			case 490:
			case 491:
			case 552:
			case 632:
				return set[23];
			case 57:
			case 78:
			case 158:
				return set[24];
			case 58:
				return set[25];
			case 59:
				{
					BitArray a = new BitArray(239);
					a.Set(216, true);
					return a;
				}
			case 60:
				{
					BitArray a = new BitArray(239);
					a.Set(145, true);
					return a;
				}
			case 61:
			case 156:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 62:
				{
					BitArray a = new BitArray(239);
					a.Set(236, true);
					return a;
				}
			case 63:
				{
					BitArray a = new BitArray(239);
					a.Set(177, true);
					return a;
				}
			case 64:
				{
					BitArray a = new BitArray(239);
					a.Set(175, true);
					return a;
				}
			case 65:
				{
					BitArray a = new BitArray(239);
					a.Set(61, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(60, true);
					return a;
				}
			case 67:
				{
					BitArray a = new BitArray(239);
					a.Set(150, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(42, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(43, true);
					return a;
				}
			case 71:
			case 445:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 72:
				{
					BitArray a = new BitArray(239);
					a.Set(41, true);
					return a;
				}
			case 73:
			case 98:
			case 222:
			case 223:
			case 284:
			case 285:
			case 336:
			case 742:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 74:
				{
					BitArray a = new BitArray(239);
					a.Set(154, true);
					return a;
				}
			case 79:
			case 91:
			case 93:
			case 148:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 80:
			case 81:
				return set[26];
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 84:
			case 100:
			case 519:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 85:
			case 121:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 86:
			case 87:
				return set[27];
			case 89:
			case 92:
			case 149:
			case 150:
			case 153:
				return set[28];
			case 90:
			case 102:
			case 147:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(36, true);
					a.Set(147, true);
					return a;
				}
			case 95:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 96:
			case 689:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 103:
			case 355:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 104:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 107:
			case 301:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 108:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 111:
			case 454:
			case 459:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 115:
			case 361:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 116:
			case 604:
			case 623:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 119:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 120:
			case 318:
			case 325:
			case 340:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 122:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 123:
			case 226:
			case 231:
			case 233:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 124:
			case 228:
			case 232:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 125:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 126:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 127:
			case 254:
			case 260:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 128:
			case 151:
			case 251:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 129:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 130:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 131:
			case 196:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 132:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 133:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 134:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 135:
			case 641:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 136:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 137:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 138:
			case 208:
			case 238:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 139:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 140:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 141:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 142:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 143:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 144:
			case 250:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 145:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 146:
				return set[29];
			case 154:
				return set[30];
			case 159:
				return set[31];
			case 160:
				return set[32];
			case 161:
			case 162:
			case 483:
			case 484:
				return set[33];
			case 164:
				return set[34];
			case 173:
			case 174:
			case 321:
			case 330:
				return set[35];
			case 175:
			case 253:
			case 461:
				return set[36];
			case 176:
			case 377:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 177:
			case 184:
			case 189:
			case 255:
			case 426:
			case 455:
			case 477:
			case 480:
			case 593:
			case 594:
			case 646:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 183:
				return set[37];
			case 190:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 191:
			case 192:
				return set[38];
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 198:
			case 212:
			case 230:
			case 235:
			case 241:
			case 243:
			case 247:
			case 249:
				return set[39];
			case 200:
			case 201:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 202:
			case 204:
			case 322:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 206:
			case 207:
			case 209:
			case 211:
			case 213:
			case 214:
			case 224:
			case 229:
			case 234:
			case 242:
			case 246:
			case 271:
			case 275:
				return set[40];
			case 210:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 216:
				return set[41];
			case 219:
			case 280:
				return set[42];
			case 220:
			case 281:
				return set[43];
			case 225:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 227:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 236:
			case 237:
				return set[44];
			case 240:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 245:
				return set[45];
			case 252:
			case 556:
			case 667:
			case 678:
			case 686:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 256:
			case 257:
				return set[46];
			case 263:
			case 264:
				return set[47];
			case 265:
				return set[48];
			case 268:
				return set[49];
			case 269:
			case 270:
			case 383:
				return set[50];
			case 272:
			case 277:
			case 367:
			case 659:
			case 660:
			case 662:
			case 697:
			case 714:
			case 715:
			case 717:
			case 726:
			case 727:
			case 729:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 274:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 282:
			case 283:
				return set[51];
			case 287:
			case 329:
			case 344:
			case 406:
				return set[52];
			case 290:
			case 291:
			case 311:
			case 312:
			case 326:
			case 327:
			case 341:
			case 342:
				return set[53];
			case 292:
			case 384:
			case 387:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 293:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 294:
				return set[54];
			case 295:
			case 314:
				return set[55];
			case 296:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 299:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 303:
			case 304:
				return set[56];
			case 305:
			case 310:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 307:
			case 308:
				return set[57];
			case 313:
				return set[58];
			case 315:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 316:
			case 317:
				return set[59];
			case 319:
			case 320:
				return set[60];
			case 331:
			case 332:
				return set[61];
			case 333:
				return set[62];
			case 335:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 338:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 346:
				return set[63];
			case 347:
			case 351:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 348:
				return set[64];
			case 356:
			case 357:
				return set[65];
			case 360:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 362:
			case 363:
				return set[66];
			case 365:
			case 366:
				return set[67];
			case 368:
			case 370:
				return set[68];
			case 373:
			case 379:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 376:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 380:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 381:
			case 382:
			case 456:
			case 457:
				return set[69];
			case 385:
			case 386:
			case 388:
			case 389:
				return set[70];
			case 391:
				return set[71];
			case 400:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 402:
			case 403:
			case 408:
			case 409:
				return set[72];
			case 404:
			case 410:
				return set[73];
			case 405:
			case 413:
			case 420:
				return set[74];
			case 411:
			case 412:
			case 418:
			case 419:
			case 704:
			case 705:
				return set[75];
			case 427:
			case 428:
				return set[76];
			case 430:
			case 432:
			case 433:
			case 595:
			case 647:
				return set[77];
			case 434:
			case 435:
				return set[78];
			case 436:
			case 437:
				return set[79];
			case 438:
				return set[80];
			case 439:
			case 443:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 446:
			case 447:
			case 451:
				return set[81];
			case 448:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(39, true);
					return a;
				}
			case 449:
			case 450:
				return set[82];
			case 452:
				{
					BitArray a = new BitArray(239);
					a.Set(21, true);
					return a;
				}
			case 460:
				return set[83];
			case 462:
			case 475:
				return set[84];
			case 463:
			case 476:
				return set[85];
			case 464:
			case 465:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 470:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 472:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 473:
				return set[86];
			case 474:
				return set[87];
			case 486:
				return set[88];
			case 495:
			case 497:
				return set[89];
			case 500:
			case 501:
			case 563:
			case 564:
			case 692:
			case 693:
				return set[90];
			case 503:
			case 504:
			case 505:
			case 510:
			case 511:
			case 566:
			case 695:
			case 722:
			case 731:
				return set[91];
			case 506:
			case 512:
			case 521:
				return set[92];
			case 507:
			case 508:
			case 513:
			case 514:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 515:
			case 517:
			case 522:
				return set[93];
			case 518:
			case 520:
				return set[94];
			case 527:
			case 540:
			case 541:
			case 597:
			case 674:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 528:
			case 529:
			case 601:
			case 602:
				return set[95];
			case 537:
			case 538:
			case 545:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 543:
			case 544:
				return set[96];
			case 548:
			case 549:
				return set[97];
			case 550:
			case 551:
			case 610:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 555:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 567:
			case 568:
			case 580:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 573:
			case 574:
				return set[98];
			case 575:
			case 576:
				return set[99];
			case 577:
			case 578:
			case 589:
				return set[100];
			case 583:
			case 584:
				return set[101];
			case 585:
			case 586:
			case 711:
				return set[102];
			case 587:
				return set[103];
			case 588:
				return set[104];
			case 590:
			case 600:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 606:
			case 607:
				return set[105];
			case 608:
				return set[106];
			case 609:
			case 640:
				return set[107];
			case 612:
			case 613:
			case 614:
			case 631:
				return set[108];
			case 615:
			case 619:
			case 629:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 621:
				return set[109];
			case 624:
				return set[110];
			case 625:
				return set[111];
			case 634:
			case 635:
			case 637:
			case 703:
			case 706:
				return set[112];
			case 638:
			case 639:
				return set[113];
			case 642:
			case 644:
			case 653:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 643:
				return set[114];
			case 645:
				return set[115];
			case 651:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 655:
			case 656:
				return set[116];
			case 657:
			case 664:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 665:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 666:
				return set[117];
			case 669:
			case 670:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 671:
			case 677:
			case 743:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 672:
				return set[118];
			case 681:
			case 682:
				return set[119];
			case 683:
			case 691:
				return set[120];
			case 687:
				return set[121];
			case 696:
			case 698:
				return set[122];
			case 699:
			case 710:
				return set[123];
			case 700:
			case 701:
				return set[124];
			case 702:
				return set[125];
			case 712:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 718:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 721:
			case 730:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 735:
				return set[126];
			case 736:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 737:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 738:
			case 739:
				return set[127];
			case 744:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 745:
				return set[128];
			case 747:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 748:
				return set[129];
			case 749:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 750:
				return set[130];
			default: throw new InvalidOperationException();
		}
	}

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool wasQualifierTokenAtStart = false;
	bool nextTokenIsPotentialStartOfExpression = false;
	bool readXmlIdentifier = false;
	bool identifierExpected = false;
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	bool isMissingModifier = false;
	bool isAlreadyInExpr = false;
	bool wasNormalAttribute = false;
	int lambdaNestingDepth = 0;
	int activeArgument = 0;
	List<Token> errors = new List<Token>();
	
	public ExpressionFinder()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token la)
	{
		if (la.kind != expectedKind) {
			Error(la);
			output.AppendLine("expected: " + expectedKind);
			//Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		output.AppendLine("not expected: " + la);
		//Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 747;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 737;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 445;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (la == null) { currentState = 4; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(4);
					goto case 5;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 5: {
				if (la == null) { currentState = 5; break; }
				if (la.kind == 160) {
					currentState = 733;
					break;
				} else {
					if (set[4].Get(la.kind)) {
						goto case 7;
					} else {
						goto case 6;
					}
				}
			}
			case 6: {
				Error(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 7: {
				if (la == null) { currentState = 7; break; }
				if (la.kind == 40) {
					stateStack.Push(7);
					goto case 445;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[131].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 567;
					} else {
						if (la.kind == 103) {
							currentState = 556;
							break;
						} else {
							if (la.kind == 115) {
								goto case 537;
							} else {
								if (la.kind == 142) {
									goto case 9;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 9: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 10;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				Expect(142, la); // "Interface"
				currentState = 11;
				break;
			}
			case 11: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(12);
				goto case 205;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 730;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				stateStack.Push(15);
				goto case 23;
			}
			case 15: {
				isMissingModifier = true;
				goto case 16;
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.kind == 140) {
					currentState = 725;
					break;
				} else {
					goto case 17;
				}
			}
			case 17: {
				isMissingModifier = true;
				goto case 18;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[10].Get(la.kind)) {
					goto case 26;
				} else {
					isMissingModifier = false;
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				Expect(113, la); // "End"
				currentState = 20;
				break;
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				Expect(142, la); // "Interface"
				currentState = 21;
				break;
			}
			case 21: {
				stateStack.Push(22);
				goto case 23;
			}
			case 22: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 23: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 24;
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 26: {
				isMissingModifier = true;
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				if (la.kind == 40) {
					stateStack.Push(26);
					goto case 445;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[132].Get(la.kind)) {
					currentState = 724;
					break;
				} else {
					isMissingModifier = false;
					goto case 29;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(17);
					goto case 567;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 555;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 537;
						} else {
							if (la.kind == 142) {
								stateStack.Push(17);
								goto case 9;
							} else {
								if (set[13].Get(la.kind)) {
									stateStack.Push(17);
									goto case 30;
								} else {
									Error(la);
									goto case 17;
								}
							}
						}
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (la.kind == 119) {
					currentState = 531;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 524;
						break;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							currentState = 31;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 31: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 32;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				currentState = 33;
				break;
			}
			case 33: {
				PopContext();
				goto case 34;
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 37) {
					currentState = 500;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 35;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 35: {
				PushContext(Context.Type, la, t);
				stateStack.Push(36);
				goto case 37;
			}
			case 36: {
				PopContext();
				goto case 23;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (la.kind == 130) {
					currentState = 38;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 38;
						break;
					} else {
						if (set[133].Get(la.kind)) {
							currentState = 38;
							break;
						} else {
							if (la.kind == 33) {
								currentState = 38;
								break;
							} else {
								Error(la);
								goto case 38;
							}
						}
					}
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (la.kind == 33 || la.kind == 37) {
					stateStack.Push(38);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (la.kind == 26) {
					currentState = 40;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 40: {
				stateStack.Push(41);
				goto case 101;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.kind == 33 || la.kind == 37) {
					stateStack.Push(41);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				if (la.kind == 33) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 37) {
						currentState = 43;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 43: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 169) {
					currentState = 495;
					break;
				} else {
					if (set[22].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(45);
							goto case 47;
						} else {
							goto case 45;
						}
					} else {
						Error(la);
						goto case 45;
					}
				}
			}
			case 45: {
				PopContext();
				goto case 46;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 47: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 48;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (set[23].Get(la.kind)) {
					activeArgument = 0;
					goto case 491;
				} else {
					if (la.kind == 22) {
						activeArgument = 0;
						goto case 49;
					} else {
						goto case 6;
					}
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(22, la); // ","
				currentState = 50;
				break;
			}
			case 50: {
				activeArgument++;
				goto case 51;
			}
			case 51: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 52;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(53);
					goto case 55;
				} else {
					goto case 53;
				}
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (la.kind == 22) {
					currentState = 54;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 54: {
				activeArgument++;
				goto case 51;
			}
			case 55: {
				PushContext(Context.Expression, la, t);
				goto case 56;
			}
			case 56: {
				stateStack.Push(57);
				goto case 75;
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				if (set[25].Get(la.kind)) {
					stateStack.Push(56);
					goto case 58;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.kind == 31) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 30) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 34) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 25) {
								currentState = stateStack.Pop();
								break;
							} else {
								if (la.kind == 24) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 32) {
										currentState = stateStack.Pop();
										break;
									} else {
										if (la.kind == 154) {
											goto case 74;
										} else {
											if (la.kind == 20) {
												goto case 73;
											} else {
												if (la.kind == 41) {
													goto case 72;
												} else {
													if (la.kind == 40) {
														goto case 71;
													} else {
														if (la.kind == 39) {
															currentState = 70;
															break;
														} else {
															if (la.kind == 43) {
																goto case 69;
															} else {
																if (la.kind == 42) {
																	goto case 68;
																} else {
																	if (la.kind == 150) {
																		goto case 67;
																	} else {
																		if (la.kind == 23) {
																			currentState = stateStack.Pop();
																			break;
																		} else {
																			if (la.kind == 60) {
																				goto case 66;
																			} else {
																				if (la.kind == 61) {
																					goto case 65;
																				} else {
																					if (la.kind == 175) {
																						goto case 64;
																					} else {
																						if (la.kind == 177) {
																							goto case 63;
																						} else {
																							if (la.kind == 236) {
																								goto case 62;
																							} else {
																								if (la.kind == 44) {
																									currentState = stateStack.Pop();
																									break;
																								} else {
																									if (la.kind == 45) {
																										currentState = stateStack.Pop();
																										break;
																									} else {
																										if (la.kind == 144) {
																											goto case 61;
																										} else {
																											if (la.kind == 145) {
																												goto case 60;
																											} else {
																												if (la.kind == 47) {
																													currentState = stateStack.Pop();
																													break;
																												} else {
																													if (la.kind == 49) {
																														currentState = stateStack.Pop();
																														break;
																													} else {
																														if (la.kind == 50) {
																															currentState = stateStack.Pop();
																															break;
																														} else {
																															if (la.kind == 51) {
																																currentState = stateStack.Pop();
																																break;
																															} else {
																																if (la.kind == 46) {
																																	currentState = stateStack.Pop();
																																	break;
																																} else {
																																	if (la.kind == 48) {
																																		currentState = stateStack.Pop();
																																		break;
																																	} else {
																																		if (la.kind == 54) {
																																			currentState = stateStack.Pop();
																																			break;
																																		} else {
																																			if (la.kind == 52) {
																																				currentState = stateStack.Pop();
																																				break;
																																			} else {
																																				if (la.kind == 53) {
																																					currentState = stateStack.Pop();
																																					break;
																																				} else {
																																					if (la.kind == 216) {
																																						goto case 59;
																																					} else {
																																						if (la.kind == 55) {
																																							currentState = stateStack.Pop();
																																							break;
																																						} else {
																																							goto case 6;
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 70: {
				wasNormalAttribute = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 75: {
				PushContext(Context.Expression, la, t);
				goto case 76;
			}
			case 76: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 77;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				if (set[134].Get(la.kind)) {
					currentState = 76;
					break;
				} else {
					if (set[35].Get(la.kind)) {
						stateStack.Push(159);
						goto case 173;
					} else {
						if (la.kind == 220) {
							currentState = 155;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(78);
								goto case 85;
							} else {
								if (la.kind == 35) {
									stateStack.Push(78);
									goto case 79;
								} else {
									Error(la);
									goto case 78;
								}
							}
						}
					}
				}
			}
			case 78: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				Expect(35, la); // "{"
				currentState = 80;
				break;
			}
			case 80: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 81;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (set[23].Get(la.kind)) {
					goto case 83;
				} else {
					goto case 82;
				}
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 83: {
				stateStack.Push(84);
				goto case 55;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.kind == 22) {
					currentState = 83;
					break;
				} else {
					goto case 82;
				}
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				Expect(162, la); // "New"
				currentState = 86;
				break;
			}
			case 86: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 87;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(146);
					goto case 37;
				} else {
					if (la.kind == 233) {
						PushContext(Context.ObjectInitializer, la, t);
						goto case 90;
					} else {
						goto case 88;
					}
				}
			}
			case 88: {
				Error(la);
				goto case 89;
			}
			case 89: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				Expect(233, la); // "With"
				currentState = 91;
				break;
			}
			case 91: {
				stateStack.Push(92);
				goto case 93;
			}
			case 92: {
				PopContext();
				goto case 89;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				Expect(35, la); // "{"
				currentState = 94;
				break;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				if (la.kind == 26 || la.kind == 147) {
					goto case 95;
				} else {
					goto case 82;
				}
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				if (la.kind == 147) {
					currentState = 96;
					break;
				} else {
					goto case 96;
				}
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				Expect(26, la); // "."
				currentState = 97;
				break;
			}
			case 97: {
				stateStack.Push(98);
				goto case 101;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				Expect(20, la); // "="
				currentState = 99;
				break;
			}
			case 99: {
				stateStack.Push(100);
				goto case 55;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				if (la.kind == 22) {
					currentState = 95;
					break;
				} else {
					goto case 82;
				}
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (la.kind == 2) {
					goto case 145;
				} else {
					if (la.kind == 56) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 57) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 58) {
								goto case 144;
							} else {
								if (la.kind == 59) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 60) {
										goto case 66;
									} else {
										if (la.kind == 61) {
											goto case 65;
										} else {
											if (la.kind == 62) {
												goto case 143;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 142;
													} else {
														if (la.kind == 65) {
															goto case 141;
														} else {
															if (la.kind == 66) {
																goto case 140;
															} else {
																if (la.kind == 67) {
																	goto case 139;
																} else {
																	if (la.kind == 68) {
																		currentState = stateStack.Pop();
																		break;
																	} else {
																		if (la.kind == 69) {
																			currentState = stateStack.Pop();
																			break;
																		} else {
																			if (la.kind == 70) {
																				goto case 138;
																			} else {
																				if (la.kind == 71) {
																					currentState = stateStack.Pop();
																					break;
																				} else {
																					if (la.kind == 72) {
																						currentState = stateStack.Pop();
																						break;
																					} else {
																						if (la.kind == 73) {
																							currentState = stateStack.Pop();
																							break;
																						} else {
																							if (la.kind == 74) {
																								currentState = stateStack.Pop();
																								break;
																							} else {
																								if (la.kind == 75) {
																									currentState = stateStack.Pop();
																									break;
																								} else {
																									if (la.kind == 76) {
																										currentState = stateStack.Pop();
																										break;
																									} else {
																										if (la.kind == 77) {
																											currentState = stateStack.Pop();
																											break;
																										} else {
																											if (la.kind == 78) {
																												currentState = stateStack.Pop();
																												break;
																											} else {
																												if (la.kind == 79) {
																													currentState = stateStack.Pop();
																													break;
																												} else {
																													if (la.kind == 80) {
																														currentState = stateStack.Pop();
																														break;
																													} else {
																														if (la.kind == 81) {
																															currentState = stateStack.Pop();
																															break;
																														} else {
																															if (la.kind == 82) {
																																currentState = stateStack.Pop();
																																break;
																															} else {
																																if (la.kind == 83) {
																																	currentState = stateStack.Pop();
																																	break;
																																} else {
																																	if (la.kind == 84) {
																																		goto case 137;
																																	} else {
																																		if (la.kind == 85) {
																																			currentState = stateStack.Pop();
																																			break;
																																		} else {
																																			if (la.kind == 86) {
																																				currentState = stateStack.Pop();
																																				break;
																																			} else {
																																				if (la.kind == 87) {
																																					goto case 136;
																																				} else {
																																					if (la.kind == 88) {
																																						currentState = stateStack.Pop();
																																						break;
																																					} else {
																																						if (la.kind == 89) {
																																							currentState = stateStack.Pop();
																																							break;
																																						} else {
																																							if (la.kind == 90) {
																																								currentState = stateStack.Pop();
																																								break;
																																							} else {
																																								if (la.kind == 91) {
																																									currentState = stateStack.Pop();
																																									break;
																																								} else {
																																									if (la.kind == 92) {
																																										currentState = stateStack.Pop();
																																										break;
																																									} else {
																																										if (la.kind == 93) {
																																											currentState = stateStack.Pop();
																																											break;
																																										} else {
																																											if (la.kind == 94) {
																																												currentState = stateStack.Pop();
																																												break;
																																											} else {
																																												if (la.kind == 95) {
																																													currentState = stateStack.Pop();
																																													break;
																																												} else {
																																													if (la.kind == 96) {
																																														currentState = stateStack.Pop();
																																														break;
																																													} else {
																																														if (la.kind == 97) {
																																															currentState = stateStack.Pop();
																																															break;
																																														} else {
																																															if (la.kind == 98) {
																																																goto case 135;
																																															} else {
																																																if (la.kind == 99) {
																																																	currentState = stateStack.Pop();
																																																	break;
																																																} else {
																																																	if (la.kind == 100) {
																																																		currentState = stateStack.Pop();
																																																		break;
																																																	} else {
																																																		if (la.kind == 101) {
																																																			currentState = stateStack.Pop();
																																																			break;
																																																		} else {
																																																			if (la.kind == 102) {
																																																				currentState = stateStack.Pop();
																																																				break;
																																																			} else {
																																																				if (la.kind == 103) {
																																																					currentState = stateStack.Pop();
																																																					break;
																																																				} else {
																																																					if (la.kind == 104) {
																																																						goto case 134;
																																																					} else {
																																																						if (la.kind == 105) {
																																																							currentState = stateStack.Pop();
																																																							break;
																																																						} else {
																																																							if (la.kind == 106) {
																																																								currentState = stateStack.Pop();
																																																								break;
																																																							} else {
																																																								if (la.kind == 107) {
																																																									goto case 133;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 132;
																																																									} else {
																																																										if (la.kind == 109) {
																																																											currentState = stateStack.Pop();
																																																											break;
																																																										} else {
																																																											if (la.kind == 110) {
																																																												currentState = stateStack.Pop();
																																																												break;
																																																											} else {
																																																												if (la.kind == 111) {
																																																													currentState = stateStack.Pop();
																																																													break;
																																																												} else {
																																																													if (la.kind == 112) {
																																																														currentState = stateStack.Pop();
																																																														break;
																																																													} else {
																																																														if (la.kind == 113) {
																																																															currentState = stateStack.Pop();
																																																															break;
																																																														} else {
																																																															if (la.kind == 114) {
																																																																currentState = stateStack.Pop();
																																																																break;
																																																															} else {
																																																																if (la.kind == 115) {
																																																																	currentState = stateStack.Pop();
																																																																	break;
																																																																} else {
																																																																	if (la.kind == 116) {
																																																																		goto case 131;
																																																																	} else {
																																																																		if (la.kind == 117) {
																																																																			currentState = stateStack.Pop();
																																																																			break;
																																																																		} else {
																																																																			if (la.kind == 118) {
																																																																				currentState = stateStack.Pop();
																																																																				break;
																																																																			} else {
																																																																				if (la.kind == 119) {
																																																																					currentState = stateStack.Pop();
																																																																					break;
																																																																				} else {
																																																																					if (la.kind == 120) {
																																																																						currentState = stateStack.Pop();
																																																																						break;
																																																																					} else {
																																																																						if (la.kind == 121) {
																																																																							goto case 130;
																																																																						} else {
																																																																							if (la.kind == 122) {
																																																																								currentState = stateStack.Pop();
																																																																								break;
																																																																							} else {
																																																																								if (la.kind == 123) {
																																																																									currentState = stateStack.Pop();
																																																																									break;
																																																																								} else {
																																																																									if (la.kind == 124) {
																																																																										goto case 129;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 128;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 127;
																																																																												} else {
																																																																													if (la.kind == 128) {
																																																																														currentState = stateStack.Pop();
																																																																														break;
																																																																													} else {
																																																																														if (la.kind == 129) {
																																																																															currentState = stateStack.Pop();
																																																																															break;
																																																																														} else {
																																																																															if (la.kind == 130) {
																																																																																currentState = stateStack.Pop();
																																																																																break;
																																																																															} else {
																																																																																if (la.kind == 131) {
																																																																																	currentState = stateStack.Pop();
																																																																																	break;
																																																																																} else {
																																																																																	if (la.kind == 132) {
																																																																																		currentState = stateStack.Pop();
																																																																																		break;
																																																																																	} else {
																																																																																		if (la.kind == 133) {
																																																																																			goto case 126;
																																																																																		} else {
																																																																																			if (la.kind == 134) {
																																																																																				currentState = stateStack.Pop();
																																																																																				break;
																																																																																			} else {
																																																																																				if (la.kind == 135) {
																																																																																					currentState = stateStack.Pop();
																																																																																					break;
																																																																																				} else {
																																																																																					if (la.kind == 136) {
																																																																																						currentState = stateStack.Pop();
																																																																																						break;
																																																																																					} else {
																																																																																						if (la.kind == 137) {
																																																																																							currentState = stateStack.Pop();
																																																																																							break;
																																																																																						} else {
																																																																																							if (la.kind == 138) {
																																																																																								currentState = stateStack.Pop();
																																																																																								break;
																																																																																							} else {
																																																																																								if (la.kind == 139) {
																																																																																									goto case 125;
																																																																																								} else {
																																																																																									if (la.kind == 140) {
																																																																																										currentState = stateStack.Pop();
																																																																																										break;
																																																																																									} else {
																																																																																										if (la.kind == 141) {
																																																																																											currentState = stateStack.Pop();
																																																																																											break;
																																																																																										} else {
																																																																																											if (la.kind == 142) {
																																																																																												currentState = stateStack.Pop();
																																																																																												break;
																																																																																											} else {
																																																																																												if (la.kind == 143) {
																																																																																													goto case 124;
																																																																																												} else {
																																																																																													if (la.kind == 144) {
																																																																																														goto case 61;
																																																																																													} else {
																																																																																														if (la.kind == 145) {
																																																																																															goto case 60;
																																																																																														} else {
																																																																																															if (la.kind == 146) {
																																																																																																goto case 123;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 122;
																																																																																																} else {
																																																																																																	if (la.kind == 148) {
																																																																																																		currentState = stateStack.Pop();
																																																																																																		break;
																																																																																																	} else {
																																																																																																		if (la.kind == 149) {
																																																																																																			currentState = stateStack.Pop();
																																																																																																			break;
																																																																																																		} else {
																																																																																																			if (la.kind == 150) {
																																																																																																				goto case 67;
																																																																																																			} else {
																																																																																																				if (la.kind == 151) {
																																																																																																					currentState = stateStack.Pop();
																																																																																																					break;
																																																																																																				} else {
																																																																																																					if (la.kind == 152) {
																																																																																																						currentState = stateStack.Pop();
																																																																																																						break;
																																																																																																					} else {
																																																																																																						if (la.kind == 153) {
																																																																																																							currentState = stateStack.Pop();
																																																																																																							break;
																																																																																																						} else {
																																																																																																							if (la.kind == 154) {
																																																																																																								goto case 74;
																																																																																																							} else {
																																																																																																								if (la.kind == 155) {
																																																																																																									currentState = stateStack.Pop();
																																																																																																									break;
																																																																																																								} else {
																																																																																																									if (la.kind == 156) {
																																																																																																										currentState = stateStack.Pop();
																																																																																																										break;
																																																																																																									} else {
																																																																																																										if (la.kind == 157) {
																																																																																																											currentState = stateStack.Pop();
																																																																																																											break;
																																																																																																										} else {
																																																																																																											if (la.kind == 158) {
																																																																																																												currentState = stateStack.Pop();
																																																																																																												break;
																																																																																																											} else {
																																																																																																												if (la.kind == 159) {
																																																																																																													currentState = stateStack.Pop();
																																																																																																													break;
																																																																																																												} else {
																																																																																																													if (la.kind == 160) {
																																																																																																														currentState = stateStack.Pop();
																																																																																																														break;
																																																																																																													} else {
																																																																																																														if (la.kind == 161) {
																																																																																																															currentState = stateStack.Pop();
																																																																																																															break;
																																																																																																														} else {
																																																																																																															if (la.kind == 162) {
																																																																																																																goto case 121;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 120;
																																																																																																																} else {
																																																																																																																	if (la.kind == 164) {
																																																																																																																		currentState = stateStack.Pop();
																																																																																																																		break;
																																																																																																																	} else {
																																																																																																																		if (la.kind == 165) {
																																																																																																																			currentState = stateStack.Pop();
																																																																																																																			break;
																																																																																																																		} else {
																																																																																																																			if (la.kind == 166) {
																																																																																																																				currentState = stateStack.Pop();
																																																																																																																				break;
																																																																																																																			} else {
																																																																																																																				if (la.kind == 167) {
																																																																																																																					currentState = stateStack.Pop();
																																																																																																																					break;
																																																																																																																				} else {
																																																																																																																					if (la.kind == 168) {
																																																																																																																						currentState = stateStack.Pop();
																																																																																																																						break;
																																																																																																																					} else {
																																																																																																																						if (la.kind == 169) {
																																																																																																																							currentState = stateStack.Pop();
																																																																																																																							break;
																																																																																																																						} else {
																																																																																																																							if (la.kind == 170) {
																																																																																																																								goto case 119;
																																																																																																																							} else {
																																																																																																																								if (la.kind == 171) {
																																																																																																																									currentState = stateStack.Pop();
																																																																																																																									break;
																																																																																																																								} else {
																																																																																																																									if (la.kind == 172) {
																																																																																																																										currentState = stateStack.Pop();
																																																																																																																										break;
																																																																																																																									} else {
																																																																																																																										if (la.kind == 173) {
																																																																																																																											currentState = stateStack.Pop();
																																																																																																																											break;
																																																																																																																										} else {
																																																																																																																											if (la.kind == 174) {
																																																																																																																												currentState = stateStack.Pop();
																																																																																																																												break;
																																																																																																																											} else {
																																																																																																																												if (la.kind == 175) {
																																																																																																																													goto case 64;
																																																																																																																												} else {
																																																																																																																													if (la.kind == 176) {
																																																																																																																														goto case 118;
																																																																																																																													} else {
																																																																																																																														if (la.kind == 177) {
																																																																																																																															goto case 63;
																																																																																																																														} else {
																																																																																																																															if (la.kind == 178) {
																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																break;
																																																																																																																															} else {
																																																																																																																																if (la.kind == 179) {
																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																	break;
																																																																																																																																} else {
																																																																																																																																	if (la.kind == 180) {
																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																		break;
																																																																																																																																	} else {
																																																																																																																																		if (la.kind == 181) {
																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																			break;
																																																																																																																																		} else {
																																																																																																																																			if (la.kind == 182) {
																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																				break;
																																																																																																																																			} else {
																																																																																																																																				if (la.kind == 183) {
																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																					break;
																																																																																																																																				} else {
																																																																																																																																					if (la.kind == 184) {
																																																																																																																																						goto case 117;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 116;
																																																																																																																																							} else {
																																																																																																																																								if (la.kind == 187) {
																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																									break;
																																																																																																																																								} else {
																																																																																																																																									if (la.kind == 188) {
																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																										break;
																																																																																																																																									} else {
																																																																																																																																										if (la.kind == 189) {
																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																											break;
																																																																																																																																										} else {
																																																																																																																																											if (la.kind == 190) {
																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																												break;
																																																																																																																																											} else {
																																																																																																																																												if (la.kind == 191) {
																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																													break;
																																																																																																																																												} else {
																																																																																																																																													if (la.kind == 192) {
																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																														break;
																																																																																																																																													} else {
																																																																																																																																														if (la.kind == 193) {
																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																															break;
																																																																																																																																														} else {
																																																																																																																																															if (la.kind == 194) {
																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																break;
																																																																																																																																															} else {
																																																																																																																																																if (la.kind == 195) {
																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																	break;
																																																																																																																																																} else {
																																																																																																																																																	if (la.kind == 196) {
																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																		break;
																																																																																																																																																	} else {
																																																																																																																																																		if (la.kind == 197) {
																																																																																																																																																			goto case 115;
																																																																																																																																																		} else {
																																																																																																																																																			if (la.kind == 198) {
																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																				break;
																																																																																																																																																			} else {
																																																																																																																																																				if (la.kind == 199) {
																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																					break;
																																																																																																																																																				} else {
																																																																																																																																																					if (la.kind == 200) {
																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																						break;
																																																																																																																																																					} else {
																																																																																																																																																						if (la.kind == 201) {
																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																							break;
																																																																																																																																																						} else {
																																																																																																																																																							if (la.kind == 202) {
																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																								break;
																																																																																																																																																							} else {
																																																																																																																																																								if (la.kind == 203) {
																																																																																																																																																									goto case 114;
																																																																																																																																																								} else {
																																																																																																																																																									if (la.kind == 204) {
																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																										break;
																																																																																																																																																									} else {
																																																																																																																																																										if (la.kind == 205) {
																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																											break;
																																																																																																																																																										} else {
																																																																																																																																																											if (la.kind == 206) {
																																																																																																																																																												goto case 113;
																																																																																																																																																											} else {
																																																																																																																																																												if (la.kind == 207) {
																																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																																													break;
																																																																																																																																																												} else {
																																																																																																																																																													if (la.kind == 208) {
																																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																																														break;
																																																																																																																																																													} else {
																																																																																																																																																														if (la.kind == 209) {
																																																																																																																																																															goto case 112;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 111;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 110;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 109;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 108;
																																																																																																																																																																		} else {
																																																																																																																																																																			if (la.kind == 214) {
																																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																																				break;
																																																																																																																																																																			} else {
																																																																																																																																																																				if (la.kind == 215) {
																																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																																					break;
																																																																																																																																																																				} else {
																																																																																																																																																																					if (la.kind == 216) {
																																																																																																																																																																						goto case 59;
																																																																																																																																																																					} else {
																																																																																																																																																																						if (la.kind == 217) {
																																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																																							break;
																																																																																																																																																																						} else {
																																																																																																																																																																							if (la.kind == 218) {
																																																																																																																																																																								goto case 107;
																																																																																																																																																																							} else {
																																																																																																																																																																								if (la.kind == 219) {
																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																									break;
																																																																																																																																																																								} else {
																																																																																																																																																																									if (la.kind == 220) {
																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																										break;
																																																																																																																																																																									} else {
																																																																																																																																																																										if (la.kind == 221) {
																																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																																											break;
																																																																																																																																																																										} else {
																																																																																																																																																																											if (la.kind == 222) {
																																																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																																																												break;
																																																																																																																																																																											} else {
																																																																																																																																																																												if (la.kind == 223) {
																																																																																																																																																																													goto case 106;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 105;
																																																																																																																																																																													} else {
																																																																																																																																																																														if (la.kind == 225) {
																																																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																																																															break;
																																																																																																																																																																														} else {
																																																																																																																																																																															if (la.kind == 226) {
																																																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																																																break;
																																																																																																																																																																															} else {
																																																																																																																																																																																if (la.kind == 227) {
																																																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																																																	break;
																																																																																																																																																																																} else {
																																																																																																																																																																																	if (la.kind == 228) {
																																																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																																																		break;
																																																																																																																																																																																	} else {
																																																																																																																																																																																		if (la.kind == 229) {
																																																																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																																																																			break;
																																																																																																																																																																																		} else {
																																																																																																																																																																																			if (la.kind == 230) {
																																																																																																																																																																																				goto case 104;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 103;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 102;
																																																																																																																																																																																						} else {
																																																																																																																																																																																							if (la.kind == 234) {
																																																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																																																								break;
																																																																																																																																																																																							} else {
																																																																																																																																																																																								if (la.kind == 235) {
																																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																																									break;
																																																																																																																																																																																								} else {
																																																																																																																																																																																									if (la.kind == 236) {
																																																																																																																																																																																										goto case 62;
																																																																																																																																																																																									} else {
																																																																																																																																																																																										if (la.kind == 237) {
																																																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																																																											break;
																																																																																																																																																																																										} else {
																																																																																																																																																																																											goto case 6;
																																																																																																																																																																																										}
																																																																																																																																																																																									}
																																																																																																																																																																																								}
																																																																																																																																																																																							}
																																																																																																																																																																																						}
																																																																																																																																																																																					}
																																																																																																																																																																																				}
																																																																																																																																																																																			}
																																																																																																																																																																																		}
																																																																																																																																																																																	}
																																																																																																																																																																																}
																																																																																																																																																																															}
																																																																																																																																																																														}
																																																																																																																																																																													}
																																																																																																																																																																												}
																																																																																																																																																																											}
																																																																																																																																																																										}
																																																																																																																																																																									}
																																																																																																																																																																								}
																																																																																																																																																																							}
																																																																																																																																																																						}
																																																																																																																																																																					}
																																																																																																																																																																				}
																																																																																																																																																																			}
																																																																																																																																																																		}
																																																																																																																																																																	}
																																																																																																																																																																}
																																																																																																																																																															}
																																																																																																																																																														}
																																																																																																																																																													}
																																																																																																																																																												}
																																																																																																																																																											}
																																																																																																																																																										}
																																																																																																																																																									}
																																																																																																																																																								}
																																																																																																																																																							}
																																																																																																																																																						}
																																																																																																																																																					}
																																																																																																																																																				}
																																																																																																																																																			}
																																																																																																																																																		}
																																																																																																																																																	}
																																																																																																																																																}
																																																																																																																																															}
																																																																																																																																														}
																																																																																																																																													}
																																																																																																																																												}
																																																																																																																																											}
																																																																																																																																										}
																																																																																																																																									}
																																																																																																																																								}
																																																																																																																																							}
																																																																																																																																						}
																																																																																																																																					}
																																																																																																																																				}
																																																																																																																																			}
																																																																																																																																		}
																																																																																																																																	}
																																																																																																																																}
																																																																																																																															}
																																																																																																																														}
																																																																																																																													}
																																																																																																																												}
																																																																																																																											}
																																																																																																																										}
																																																																																																																									}
																																																																																																																								}
																																																																																																																							}
																																																																																																																						}
																																																																																																																					}
																																																																																																																				}
																																																																																																																			}
																																																																																																																		}
																																																																																																																	}
																																																																																																																}
																																																																																																															}
																																																																																																														}
																																																																																																													}
																																																																																																												}
																																																																																																											}
																																																																																																										}
																																																																																																									}
																																																																																																								}
																																																																																																							}
																																																																																																						}
																																																																																																					}
																																																																																																				}
																																																																																																			}
																																																																																																		}
																																																																																																	}
																																																																																																}
																																																																																															}
																																																																																														}
																																																																																													}
																																																																																												}
																																																																																											}
																																																																																										}
																																																																																									}
																																																																																								}
																																																																																							}
																																																																																						}
																																																																																					}
																																																																																				}
																																																																																			}
																																																																																		}
																																																																																	}
																																																																																}
																																																																															}
																																																																														}
																																																																													}
																																																																												}
																																																																											}
																																																																										}
																																																																									}
																																																																								}
																																																																							}
																																																																						}
																																																																					}
																																																																				}
																																																																			}
																																																																		}
																																																																	}
																																																																}
																																																															}
																																																														}
																																																													}
																																																												}
																																																											}
																																																										}
																																																									}
																																																								}
																																																							}
																																																						}
																																																					}
																																																				}
																																																			}
																																																		}
																																																	}
																																																}
																																															}
																																														}
																																													}
																																												}
																																											}
																																										}
																																									}
																																								}
																																							}
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (la.kind == 35 || la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						PushContext(Context.CollectionInitializer, la, t);
						goto case 151;
					} else {
						if (la.kind == 35) {
							PushContext(Context.CollectionInitializer, la, t);
							stateStack.Push(150);
							goto case 79;
						} else {
							if (la.kind == 233) {
								PushContext(Context.ObjectInitializer, la, t);
								goto case 147;
							} else {
								goto case 88;
							}
						}
					}
				} else {
					goto case 89;
				}
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				Expect(233, la); // "With"
				currentState = 148;
				break;
			}
			case 148: {
				stateStack.Push(149);
				goto case 93;
			}
			case 149: {
				PopContext();
				goto case 89;
			}
			case 150: {
				PopContext();
				goto case 89;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(126, la); // "From"
				currentState = 152;
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.kind == 35) {
					stateStack.Push(153);
					goto case 79;
				} else {
					if (set[30].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						Error(la);
						goto case 153;
					}
				}
			}
			case 153: {
				PopContext();
				goto case 89;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				currentState = 153;
				break;
			}
			case 155: {
				stateStack.Push(156);
				goto case 75;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(144, la); // "Is"
				currentState = 157;
				break;
			}
			case 157: {
				PushContext(Context.Type, la, t);
				stateStack.Push(158);
				goto case 37;
			}
			case 158: {
				PopContext();
				goto case 78;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (set[32].Get(la.kind)) {
					stateStack.Push(159);
					goto case 160;
				} else {
					goto case 78;
				}
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.kind == 37) {
					currentState = 165;
					break;
				} else {
					if (set[135].Get(la.kind)) {
						currentState = 161;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 161: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 162;
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				if (la.kind == 10) {
					currentState = 163;
					break;
				} else {
					goto case 163;
				}
			}
			case 163: {
				stateStack.Push(164);
				goto case 101;
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 165: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 166;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				if (la.kind == 169) {
					currentState = 168;
					break;
				} else {
					if (set[22].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(167);
							goto case 47;
						} else {
							goto case 167;
						}
					} else {
						Error(la);
						goto case 167;
					}
				}
			}
			case 167: {
				PopContext();
				goto case 46;
			}
			case 168: {
				PushContext(Context.Type, la, t);
				stateStack.Push(169);
				goto case 37;
			}
			case 169: {
				PopContext();
				goto case 170;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (la.kind == 22) {
					currentState = 171;
					break;
				} else {
					goto case 167;
				}
			}
			case 171: {
				PushContext(Context.Type, la, t);
				stateStack.Push(172);
				goto case 37;
			}
			case 172: {
				PopContext();
				goto case 170;
			}
			case 173: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 174;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (set[136].Get(la.kind)) {
					currentState = 175;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 487;
						break;
					} else {
						if (set[137].Get(la.kind)) {
							currentState = 175;
							break;
						} else {
							if (set[133].Get(la.kind)) {
								currentState = 175;
								break;
							} else {
								if (set[135].Get(la.kind)) {
									currentState = 483;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 480;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 477;
											break;
										} else {
											if (set[83].Get(la.kind)) {
												stateStack.Push(175);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 460;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(175);
													lambdaNestingDepth++;
													goto case 252;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(175);
														PushContext(Context.Query, la, t);
														goto case 190;
													} else {
														if (set[37].Get(la.kind)) {
															stateStack.Push(175);
															goto case 183;
														} else {
															if (la.kind == 135) {
																stateStack.Push(175);
																goto case 176;
															} else {
																Error(la);
																goto case 175;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 175: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(135, la); // "If"
				currentState = 177;
				break;
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				Expect(37, la); // "("
				currentState = 178;
				break;
			}
			case 178: {
				stateStack.Push(179);
				goto case 55;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				Expect(22, la); // ","
				currentState = 180;
				break;
			}
			case 180: {
				stateStack.Push(181);
				goto case 55;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				if (la.kind == 22) {
					currentState = 182;
					break;
				} else {
					goto case 46;
				}
			}
			case 182: {
				stateStack.Push(46);
				goto case 55;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (set[138].Get(la.kind)) {
					currentState = 189;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 184;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				Expect(37, la); // "("
				currentState = 185;
				break;
			}
			case 185: {
				stateStack.Push(186);
				goto case 55;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(22, la); // ","
				currentState = 187;
				break;
			}
			case 187: {
				PushContext(Context.Type, la, t);
				stateStack.Push(188);
				goto case 37;
			}
			case 188: {
				PopContext();
				goto case 46;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				Expect(37, la); // "("
				currentState = 182;
				break;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 126) {
					stateStack.Push(191);
					goto case 251;
				} else {
					if (la.kind == 58) {
						stateStack.Push(191);
						goto case 250;
					} else {
						Error(la);
						goto case 191;
					}
				}
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (set[38].Get(la.kind)) {
					stateStack.Push(191);
					goto case 192;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (la.kind == 126) {
					currentState = 248;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 244;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 242;
							break;
						} else {
							if (la.kind == 107) {
								goto case 133;
							} else {
								if (la.kind == 230) {
									currentState = 55;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 238;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 236;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 234;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 206;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 193;
														break;
													} else {
														goto case 6;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 193: {
				stateStack.Push(194);
				goto case 199;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				Expect(171, la); // "On"
				currentState = 195;
				break;
			}
			case 195: {
				stateStack.Push(196);
				goto case 55;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				Expect(116, la); // "Equals"
				currentState = 197;
				break;
			}
			case 197: {
				stateStack.Push(198);
				goto case 55;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (la.kind == 22) {
					currentState = 195;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 199: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(200);
				goto case 205;
			}
			case 200: {
				PopContext();
				goto case 201;
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (la.kind == 63) {
					currentState = 203;
					break;
				} else {
					goto case 202;
				}
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				Expect(138, la); // "In"
				currentState = 55;
				break;
			}
			case 203: {
				PushContext(Context.Type, la, t);
				stateStack.Push(204);
				goto case 37;
			}
			case 204: {
				PopContext();
				goto case 202;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (set[123].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 135;
					} else {
						goto case 6;
					}
				}
			}
			case 206: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 207;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				if (la.kind == 146) {
					goto case 226;
				} else {
					if (set[40].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 209;
							break;
						} else {
							if (set[40].Get(la.kind)) {
								goto case 224;
							} else {
								Error(la);
								goto case 208;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				Expect(70, la); // "By"
				currentState = 209;
				break;
			}
			case 209: {
				stateStack.Push(210);
				goto case 213;
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (la.kind == 22) {
					currentState = 209;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 211;
					break;
				}
			}
			case 211: {
				stateStack.Push(212);
				goto case 213;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 22) {
					currentState = 211;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 213: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 214;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(217);
					goto case 205;
				} else {
					goto case 215;
				}
			}
			case 215: {
				stateStack.Push(216);
				goto case 55;
			}
			case 216: {
				if (!isAlreadyInExpr) PopContext(); isAlreadyInExpr = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 217: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[42].Get(la.kind)) {
					PopContext(); isAlreadyInExpr = true;
					goto case 219;
				} else {
					goto case 215;
				}
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 63) {
					currentState = 221;
					break;
				} else {
					if (la.kind == 20) {
						currentState = 215;
						break;
					} else {
						if (set[43].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 215;
						}
					}
				}
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				currentState = 215;
				break;
			}
			case 221: {
				PushContext(Context.Type, la, t);
				stateStack.Push(222);
				goto case 37;
			}
			case 222: {
				PopContext();
				goto case 223;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				Expect(20, la); // "="
				currentState = 215;
				break;
			}
			case 224: {
				stateStack.Push(225);
				goto case 213;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 22) {
					currentState = 224;
					break;
				} else {
					goto case 208;
				}
			}
			case 226: {
				stateStack.Push(227);
				goto case 233;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 231;
						break;
					} else {
						if (la.kind == 146) {
							goto case 226;
						} else {
							Error(la);
							goto case 227;
						}
					}
				} else {
					goto case 228;
				}
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				Expect(143, la); // "Into"
				currentState = 229;
				break;
			}
			case 229: {
				stateStack.Push(230);
				goto case 213;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 22) {
					currentState = 229;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 231: {
				stateStack.Push(232);
				goto case 233;
			}
			case 232: {
				stateStack.Push(227);
				goto case 228;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				Expect(146, la); // "Join"
				currentState = 193;
				break;
			}
			case 234: {
				stateStack.Push(235);
				goto case 213;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 22) {
					currentState = 234;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 236: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 237;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.kind == 231) {
					currentState = 55;
					break;
				} else {
					goto case 55;
				}
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				Expect(70, la); // "By"
				currentState = 239;
				break;
			}
			case 239: {
				stateStack.Push(240);
				goto case 55;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.kind == 64) {
					currentState = 241;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 241;
						break;
					} else {
						Error(la);
						goto case 241;
					}
				}
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (la.kind == 22) {
					currentState = 239;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 242: {
				stateStack.Push(243);
				goto case 213;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (la.kind == 22) {
					currentState = 242;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 244: {
				stateStack.Push(245);
				goto case 199;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (set[38].Get(la.kind)) {
					stateStack.Push(245);
					goto case 192;
				} else {
					Expect(143, la); // "Into"
					currentState = 246;
					break;
				}
			}
			case 246: {
				stateStack.Push(247);
				goto case 213;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 22) {
					currentState = 246;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 248: {
				stateStack.Push(249);
				goto case 199;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 22) {
					currentState = 248;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				Expect(58, la); // "Aggregate"
				currentState = 244;
				break;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(126, la); // "From"
				currentState = 248;
				break;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 210) {
					stateStack.Push(253);
					goto case 454;
				} else {
					if (la.kind == 127) {
						stateStack.Push(253);
						goto case 254;
					} else {
						Error(la);
						goto case 253;
					}
				}
			}
			case 253: {
				lambdaNestingDepth--;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				Expect(127, la); // "Function"
				currentState = 255;
				break;
			}
			case 255: {
				stateStack.Push(256);
				goto case 426;
			}
			case 256: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 257;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (set[23].Get(la.kind)) {
					goto case 55;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 424;
							break;
						} else {
							goto case 258;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 258: {
				stateStack.Push(259);
				goto case 261;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				Expect(113, la); // "End"
				currentState = 260;
				break;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 261: {
				PushContext(Context.Body, la, t);
				goto case 262;
			}
			case 262: {
				stateStack.Push(263);
				goto case 23;
			}
			case 263: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 264;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (set[139].Get(la.kind)) {
					if (set[69].Get(la.kind)) {
						if (set[50].Get(la.kind)) {
							stateStack.Push(262);
							goto case 269;
						} else {
							goto case 262;
						}
					} else {
						if (la.kind == 113) {
							currentState = 267;
							break;
						} else {
							goto case 266;
						}
					}
				} else {
					goto case 265;
				}
			}
			case 265: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 266: {
				Error(la);
				goto case 263;
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 262;
				} else {
					if (set[49].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 266;
					}
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				currentState = 263;
				break;
			}
			case 269: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 270;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 401;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 397;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 395;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 390;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 372;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 356;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 352;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 346;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 319;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 315;
																break;
															} else {
																goto case 315;
															}
														} else {
															if (la.kind == 194) {
																currentState = 313;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 311;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 298;
																break;
															} else {
																if (set[140].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 295;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 294;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 293;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 113;
																				} else {
																					if (la.kind == 195) {
																						currentState = 290;
																						break;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 288;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 286;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 271;
																				break;
																			} else {
																				if (set[141].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 55;
																						break;
																					} else {
																						goto case 55;
																					}
																				} else {
																					goto case 6;
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 271: {
				stateStack.Push(272);
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 275;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (la.kind == 22) {
					currentState = 271;
					break;
				} else {
					stateStack.Push(273);
					goto case 261;
				}
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				Expect(113, la); // "End"
				currentState = 274;
				break;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(278);
					goto case 205;
				} else {
					goto case 276;
				}
			}
			case 276: {
				stateStack.Push(277);
				goto case 55;
			}
			case 277: {
				if (!isAlreadyInExpr) PopContext(); isAlreadyInExpr = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 278: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 279;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (set[42].Get(la.kind)) {
					PopContext(); isAlreadyInExpr = true;
					goto case 280;
				} else {
					goto case 276;
				}
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (la.kind == 63) {
					currentState = 282;
					break;
				} else {
					if (la.kind == 20) {
						currentState = 276;
						break;
					} else {
						if (set[43].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 276;
						}
					}
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				currentState = 276;
				break;
			}
			case 282: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 283;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(284);
					goto case 37;
				} else {
					goto case 276;
				}
			}
			case 284: {
				PopContext();
				goto case 285;
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				Expect(20, la); // "="
				currentState = 276;
				break;
			}
			case 286: {
				stateStack.Push(287);
				goto case 55;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 22) {
					currentState = 286;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 288: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 289;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				if (la.kind == 184) {
					currentState = 55;
					break;
				} else {
					goto case 55;
				}
			}
			case 290: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 291;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(292);
					goto case 55;
				} else {
					goto case 292;
				}
			}
			case 292: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 108) {
					goto case 132;
				} else {
					if (la.kind == 124) {
						goto case 129;
					} else {
						if (la.kind == 231) {
							goto case 103;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 108) {
					goto case 132;
				} else {
					if (la.kind == 124) {
						goto case 129;
					} else {
						if (la.kind == 231) {
							goto case 103;
						} else {
							if (la.kind == 197) {
								goto case 115;
							} else {
								if (la.kind == 210) {
									goto case 111;
								} else {
									if (la.kind == 127) {
										goto case 127;
									} else {
										if (la.kind == 186) {
											goto case 116;
										} else {
											if (la.kind == 218) {
												goto case 107;
											} else {
												goto case 6;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (set[6].Get(la.kind)) {
					goto case 297;
				} else {
					if (la.kind == 5) {
						goto case 296;
					} else {
						goto case 6;
					}
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 298: {
				stateStack.Push(299);
				goto case 261;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 75) {
					currentState = 303;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 302;
						break;
					} else {
						goto case 300;
					}
				}
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				Expect(113, la); // "End"
				currentState = 301;
				break;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 302: {
				stateStack.Push(300);
				goto case 261;
			}
			case 303: {
				SetIdentifierExpected(la);
				goto case 304;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(307);
					goto case 205;
				} else {
					goto case 305;
				}
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (la.kind == 229) {
					currentState = 306;
					break;
				} else {
					goto case 298;
				}
			}
			case 306: {
				stateStack.Push(298);
				goto case 55;
			}
			case 307: {
				PopContext();
				goto case 308;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.kind == 63) {
					currentState = 309;
					break;
				} else {
					goto case 305;
				}
			}
			case 309: {
				PushContext(Context.Type, la, t);
				stateStack.Push(310);
				goto case 37;
			}
			case 310: {
				PopContext();
				goto case 305;
			}
			case 311: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 312;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (set[23].Get(la.kind)) {
					goto case 55;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 163) {
					goto case 120;
				} else {
					goto case 314;
				}
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 5) {
					goto case 296;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 297;
					} else {
						goto case 6;
					}
				}
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(118, la); // "Error"
				currentState = 316;
				break;
			}
			case 316: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 317;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (set[23].Get(la.kind)) {
					goto case 55;
				} else {
					if (la.kind == 132) {
						currentState = 314;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 318;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 319: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 320;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(336);
					goto case 330;
				} else {
					if (la.kind == 110) {
						currentState = 321;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 321: {
				stateStack.Push(322);
				goto case 330;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				Expect(138, la); // "In"
				currentState = 323;
				break;
			}
			case 323: {
				stateStack.Push(324);
				goto case 55;
			}
			case 324: {
				stateStack.Push(325);
				goto case 261;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				Expect(163, la); // "Next"
				currentState = 326;
				break;
			}
			case 326: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 327;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (set[23].Get(la.kind)) {
					goto case 328;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 328: {
				stateStack.Push(329);
				goto case 55;
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 22) {
					currentState = 328;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 330: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(331);
				goto case 173;
			}
			case 331: {
				PopContext();
				goto case 332;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 33) {
					currentState = 333;
					break;
				} else {
					goto case 333;
				}
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (set[32].Get(la.kind)) {
					stateStack.Push(333);
					goto case 160;
				} else {
					if (la.kind == 63) {
						currentState = 334;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 334: {
				PushContext(Context.Type, la, t);
				stateStack.Push(335);
				goto case 37;
			}
			case 335: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				Expect(20, la); // "="
				currentState = 337;
				break;
			}
			case 337: {
				stateStack.Push(338);
				goto case 55;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 205) {
					currentState = 345;
					break;
				} else {
					goto case 339;
				}
			}
			case 339: {
				stateStack.Push(340);
				goto case 261;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				Expect(163, la); // "Next"
				currentState = 341;
				break;
			}
			case 341: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 342;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (set[23].Get(la.kind)) {
					goto case 343;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 343: {
				stateStack.Push(344);
				goto case 55;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.kind == 22) {
					currentState = 343;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 345: {
				stateStack.Push(339);
				goto case 55;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 349;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(347);
						goto case 261;
					} else {
						goto case 6;
					}
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				Expect(152, la); // "Loop"
				currentState = 348;
				break;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 349: {
				stateStack.Push(350);
				goto case 55;
			}
			case 350: {
				stateStack.Push(351);
				goto case 261;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 352: {
				stateStack.Push(353);
				goto case 55;
			}
			case 353: {
				stateStack.Push(354);
				goto case 261;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				Expect(113, la); // "End"
				currentState = 355;
				break;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 356: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 357;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 74) {
					currentState = 358;
					break;
				} else {
					goto case 358;
				}
			}
			case 358: {
				stateStack.Push(359);
				goto case 55;
			}
			case 359: {
				stateStack.Push(360);
				goto case 23;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 74) {
					currentState = 362;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 361;
					break;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 362: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 363;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 111) {
					currentState = 364;
					break;
				} else {
					if (set[67].Get(la.kind)) {
						goto case 365;
					} else {
						Error(la);
						goto case 364;
					}
				}
			}
			case 364: {
				stateStack.Push(360);
				goto case 261;
			}
			case 365: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 366;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 368;
						break;
					} else {
						goto case 368;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(367);
						goto case 55;
					} else {
						Error(la);
						goto case 367;
					}
				}
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (la.kind == 22) {
					currentState = 365;
					break;
				} else {
					goto case 364;
				}
			}
			case 368: {
				stateStack.Push(369);
				goto case 370;
			}
			case 369: {
				stateStack.Push(367);
				goto case 75;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 20) {
					goto case 73;
				} else {
					if (la.kind == 41) {
						goto case 72;
					} else {
						if (la.kind == 40) {
							goto case 71;
						} else {
							if (la.kind == 39) {
								currentState = 371;
								break;
							} else {
								if (la.kind == 42) {
									goto case 68;
								} else {
									if (la.kind == 43) {
										goto case 69;
									} else {
										goto case 6;
									}
								}
							}
						}
					}
				}
			}
			case 371: {
				wasNormalAttribute = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 372: {
				stateStack.Push(373);
				goto case 55;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 214) {
					currentState = 381;
					break;
				} else {
					goto case 374;
				}
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 375;
				} else {
					goto case 6;
				}
			}
			case 375: {
				stateStack.Push(376);
				goto case 261;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 380;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 378;
							break;
						} else {
							Error(la);
							goto case 375;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 377;
					break;
				}
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 378: {
				stateStack.Push(379);
				goto case 55;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 214) {
					currentState = 375;
					break;
				} else {
					goto case 375;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 135) {
					currentState = 378;
					break;
				} else {
					goto case 375;
				}
			}
			case 381: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 382;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (set[50].Get(la.kind)) {
					goto case 383;
				} else {
					goto case 374;
				}
			}
			case 383: {
				stateStack.Push(384);
				goto case 269;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 21) {
					currentState = 388;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 385;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 385: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 386;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (set[50].Get(la.kind)) {
					stateStack.Push(387);
					goto case 269;
				} else {
					goto case 387;
				}
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 21) {
					currentState = 385;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 388: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 389;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (set[50].Get(la.kind)) {
					goto case 383;
				} else {
					goto case 384;
				}
			}
			case 390: {
				stateStack.Push(391);
				goto case 101;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 37) {
					currentState = 392;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 392: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 393;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(394);
					goto case 47;
				} else {
					goto case 394;
				}
			}
			case 394: {
				PopContext();
				goto case 46;
			}
			case 395: {
				stateStack.Push(396);
				goto case 55;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				Expect(22, la); // ","
				currentState = 55;
				break;
			}
			case 397: {
				stateStack.Push(398);
				goto case 55;
			}
			case 398: {
				stateStack.Push(399);
				goto case 261;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				Expect(113, la); // "End"
				currentState = 400;
				break;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (la.kind == 233) {
					goto case 102;
				} else {
					if (la.kind == 211) {
						goto case 110;
					} else {
						goto case 6;
					}
				}
			}
			case 401: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(402);
				goto case 205;
			}
			case 402: {
				PopContext();
				goto case 403;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (la.kind == 33) {
					currentState = 404;
					break;
				} else {
					goto case 404;
				}
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (la.kind == 37) {
					currentState = 421;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 418;
						break;
					} else {
						goto case 405;
					}
				}
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 20) {
					currentState = 417;
					break;
				} else {
					goto case 406;
				}
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 22) {
					currentState = 407;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 407: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(408);
				goto case 205;
			}
			case 408: {
				PopContext();
				goto case 409;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 33) {
					currentState = 410;
					break;
				} else {
					goto case 410;
				}
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				if (la.kind == 37) {
					currentState = 414;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 411;
						break;
					} else {
						goto case 405;
					}
				}
			}
			case 411: {
				PushContext(Context.Type, la, t);
				goto case 412;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 162) {
					stateStack.Push(413);
					goto case 85;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(413);
						goto case 37;
					} else {
						Error(la);
						goto case 413;
					}
				}
			}
			case 413: {
				PopContext();
				goto case 405;
			}
			case 414: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 415;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(416);
					goto case 55;
				} else {
					goto case 416;
				}
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (la.kind == 22) {
					currentState = 414;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 410;
					break;
				}
			}
			case 417: {
				stateStack.Push(406);
				goto case 55;
			}
			case 418: {
				PushContext(Context.Type, la, t);
				goto case 419;
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (la.kind == 162) {
					stateStack.Push(420);
					goto case 85;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(420);
						goto case 37;
					} else {
						Error(la);
						goto case 420;
					}
				}
			}
			case 420: {
				PopContext();
				goto case 405;
			}
			case 421: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 422;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(423);
					goto case 55;
				} else {
					goto case 423;
				}
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 22) {
					currentState = 421;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 404;
					break;
				}
			}
			case 424: {
				PushContext(Context.Type, la, t);
				stateStack.Push(425);
				goto case 37;
			}
			case 425: {
				PopContext();
				goto case 258;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				Expect(37, la); // "("
				currentState = 427;
				break;
			}
			case 427: {
				PushContext(Context.Default, la, t);
				SetIdentifierExpected(la);
				goto case 428;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(429);
					goto case 430;
				} else {
					goto case 429;
				}
			}
			case 429: {
				PopContext();
				goto case 46;
			}
			case 430: {
				stateStack.Push(431);
				PushContext(Context.Parameter, la, t);
				goto case 432;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (la.kind == 22) {
					currentState = 430;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 432: {
				SetIdentifierExpected(la);
				goto case 433;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 40) {
					stateStack.Push(432);
					goto case 445;
				} else {
					goto case 434;
				}
			}
			case 434: {
				SetIdentifierExpected(la);
				goto case 435;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (set[143].Get(la.kind)) {
					currentState = 434;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(436);
					goto case 205;
				}
			}
			case 436: {
				PopContext();
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (la.kind == 33) {
					currentState = 438;
					break;
				} else {
					goto case 438;
				}
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 37) {
					currentState = 444;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 442;
						break;
					} else {
						goto case 439;
					}
				}
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (la.kind == 20) {
					currentState = 441;
					break;
				} else {
					goto case 440;
				}
			}
			case 440: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 441: {
				stateStack.Push(440);
				goto case 55;
			}
			case 442: {
				PushContext(Context.Type, la, t);
				stateStack.Push(443);
				goto case 37;
			}
			case 443: {
				PopContext();
				goto case 439;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (la.kind == 22) {
					currentState = 444;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 438;
					break;
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				Expect(40, la); // "<"
				currentState = 446;
				break;
			}
			case 446: {
				wasNormalAttribute = true; PushContext(Context.Attribute, la, t);
				goto case 447;
			}
			case 447: {
				stateStack.Push(448);
				goto case 451;
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				if (la.kind == 22) {
					currentState = 447;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 449;
					break;
				}
			}
			case 449: {
				PopContext();
				goto case 450;
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 65 || la.kind == 155) {
					currentState = 452;
					break;
				} else {
					goto case 37;
				}
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				Expect(21, la); // ":"
				currentState = 453;
				break;
			}
			case 453: {
				wasNormalAttribute = false;
				goto case 37;
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				Expect(210, la); // "Sub"
				currentState = 455;
				break;
			}
			case 455: {
				stateStack.Push(456);
				goto case 426;
			}
			case 456: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 457;
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (set[50].Get(la.kind)) {
					goto case 269;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(458);
						goto case 261;
					} else {
						goto case 6;
					}
				}
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				Expect(113, la); // "End"
				currentState = 459;
				break;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 473;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(462);
						goto case 464;
					} else {
						Error(la);
						goto case 461;
					}
				}
			}
			case 461: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (la.kind == 17) {
					currentState = 463;
					break;
				} else {
					goto case 461;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (la.kind == 16) {
					currentState = 462;
					break;
				} else {
					goto case 462;
				}
			}
			case 464: {
				PushContext(Context.Xml, la, t);
				goto case 465;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 466;
				break;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (set[144].Get(la.kind)) {
					if (set[145].Get(la.kind)) {
						currentState = 466;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(466);
							goto case 470;
						} else {
							Error(la);
							goto case 466;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 467;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 468;
							break;
						} else {
							Error(la);
							goto case 467;
						}
					}
				}
			}
			case 467: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				if (set[146].Get(la.kind)) {
					if (set[147].Get(la.kind)) {
						currentState = 468;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(468);
							goto case 470;
						} else {
							if (la.kind == 10) {
								stateStack.Push(468);
								goto case 464;
							} else {
								Error(la);
								goto case 468;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 469;
					break;
				}
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (set[148].Get(la.kind)) {
					if (set[149].Get(la.kind)) {
						currentState = 469;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(469);
							goto case 470;
						} else {
							Error(la);
							goto case 469;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 467;
					break;
				}
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 471;
				break;
			}
			case 471: {
				stateStack.Push(472);
				goto case 55;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (la.kind == 16) {
					currentState = 474;
					break;
				} else {
					goto case 474;
				}
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 473;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(475);
						goto case 464;
					} else {
						goto case 461;
					}
				}
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (la.kind == 17) {
					currentState = 476;
					break;
				} else {
					goto case 461;
				}
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (la.kind == 16) {
					currentState = 475;
					break;
				} else {
					goto case 475;
				}
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				Expect(37, la); // "("
				currentState = 478;
				break;
			}
			case 478: {
				readXmlIdentifier = true;
				stateStack.Push(479);
				goto case 205;
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				Expect(38, la); // ")"
				currentState = 175;
				break;
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				Expect(37, la); // "("
				currentState = 481;
				break;
			}
			case 481: {
				PushContext(Context.Type, la, t);
				stateStack.Push(482);
				goto case 37;
			}
			case 482: {
				PopContext();
				goto case 479;
			}
			case 483: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 484;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 10) {
					currentState = 485;
					break;
				} else {
					goto case 485;
				}
			}
			case 485: {
				stateStack.Push(486);
				goto case 101;
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				if (la.kind == 11) {
					currentState = 175;
					break;
				} else {
					goto case 175;
				}
			}
			case 487: {
				activeArgument = 0;
				goto case 488;
			}
			case 488: {
				stateStack.Push(489);
				goto case 55;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				if (la.kind == 22) {
					currentState = 490;
					break;
				} else {
					goto case 479;
				}
			}
			case 490: {
				activeArgument++;
				goto case 488;
			}
			case 491: {
				stateStack.Push(492);
				goto case 55;
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				if (la.kind == 22) {
					currentState = 493;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 493: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (set[23].Get(la.kind)) {
					goto case 491;
				} else {
					goto case 492;
				}
			}
			case 495: {
				if (la == null) { currentState = 495; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(499);
					goto case 37;
				} else {
					goto case 496;
				}
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				if (la.kind == 22) {
					currentState = 497;
					break;
				} else {
					goto case 45;
				}
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(498);
					goto case 37;
				} else {
					goto case 496;
				}
			}
			case 498: {
				PopContext();
				goto case 496;
			}
			case 499: {
				PopContext();
				goto case 496;
			}
			case 500: {
				SetIdentifierExpected(la);
				goto case 501;
			}
			case 501: {
				if (la == null) { currentState = 501; break; }
				if (set[150].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 503;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(502);
							goto case 430;
						} else {
							Error(la);
							goto case 502;
						}
					}
				} else {
					goto case 502;
				}
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 503: {
				stateStack.Push(502);
				goto case 504;
			}
			case 504: {
				SetIdentifierExpected(la);
				goto case 505;
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 506;
					break;
				} else {
					goto case 506;
				}
			}
			case 506: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(507);
				goto case 521;
			}
			case 507: {
				PopContext();
				goto case 508;
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (la.kind == 63) {
					currentState = 522;
					break;
				} else {
					goto case 509;
				}
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				if (la.kind == 22) {
					currentState = 510;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 510: {
				SetIdentifierExpected(la);
				goto case 511;
			}
			case 511: {
				if (la == null) { currentState = 511; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 512;
					break;
				} else {
					goto case 512;
				}
			}
			case 512: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(513);
				goto case 521;
			}
			case 513: {
				PopContext();
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 63) {
					currentState = 515;
					break;
				} else {
					goto case 509;
				}
			}
			case 515: {
				PushContext(Context.Type, la, t);
				stateStack.Push(516);
				goto case 517;
			}
			case 516: {
				PopContext();
				goto case 509;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (set[94].Get(la.kind)) {
					goto case 520;
				} else {
					if (la.kind == 35) {
						currentState = 518;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 518: {
				stateStack.Push(519);
				goto case 520;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				if (la.kind == 22) {
					currentState = 518;
					break;
				} else {
					goto case 82;
				}
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (set[16].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (la.kind == 162) {
						goto case 121;
					} else {
						if (la.kind == 84) {
							goto case 137;
						} else {
							if (la.kind == 209) {
								goto case 112;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (la.kind == 2) {
					goto case 145;
				} else {
					if (la.kind == 62) {
						goto case 143;
					} else {
						if (la.kind == 64) {
							goto case 142;
						} else {
							if (la.kind == 65) {
								goto case 141;
							} else {
								if (la.kind == 66) {
									goto case 140;
								} else {
									if (la.kind == 67) {
										goto case 139;
									} else {
										if (la.kind == 70) {
											goto case 138;
										} else {
											if (la.kind == 87) {
												goto case 136;
											} else {
												if (la.kind == 104) {
													goto case 134;
												} else {
													if (la.kind == 107) {
														goto case 133;
													} else {
														if (la.kind == 116) {
															goto case 131;
														} else {
															if (la.kind == 121) {
																goto case 130;
															} else {
																if (la.kind == 133) {
																	goto case 126;
																} else {
																	if (la.kind == 139) {
																		goto case 125;
																	} else {
																		if (la.kind == 143) {
																			goto case 124;
																		} else {
																			if (la.kind == 146) {
																				goto case 123;
																			} else {
																				if (la.kind == 147) {
																					goto case 122;
																				} else {
																					if (la.kind == 170) {
																						goto case 119;
																					} else {
																						if (la.kind == 176) {
																							goto case 118;
																						} else {
																							if (la.kind == 184) {
																								goto case 117;
																							} else {
																								if (la.kind == 203) {
																									goto case 114;
																								} else {
																									if (la.kind == 212) {
																										goto case 109;
																									} else {
																										if (la.kind == 213) {
																											goto case 108;
																										} else {
																											if (la.kind == 223) {
																												goto case 106;
																											} else {
																												if (la.kind == 224) {
																													goto case 105;
																												} else {
																													if (la.kind == 230) {
																														goto case 104;
																													} else {
																														goto case 6;
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 522: {
				PushContext(Context.Type, la, t);
				stateStack.Push(523);
				goto case 517;
			}
			case 523: {
				PopContext();
				goto case 509;
			}
			case 524: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(525);
				goto case 205;
			}
			case 525: {
				PopContext();
				goto case 526;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (la.kind == 37) {
					stateStack.Push(527);
					goto case 426;
				} else {
					goto case 527;
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 63) {
					currentState = 528;
					break;
				} else {
					goto case 23;
				}
			}
			case 528: {
				PushContext(Context.Type, la, t);
				goto case 529;
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (la.kind == 40) {
					stateStack.Push(529);
					goto case 445;
				} else {
					stateStack.Push(530);
					goto case 37;
				}
			}
			case 530: {
				PopContext();
				goto case 23;
			}
			case 531: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(532);
				goto case 205;
			}
			case 532: {
				PopContext();
				goto case 533;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 535;
						break;
					} else {
						if (la.kind == 37) {
							stateStack.Push(23);
							goto case 426;
						} else {
							goto case 534;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 534: {
				Error(la);
				goto case 23;
			}
			case 535: {
				PushContext(Context.Type, la, t);
				stateStack.Push(536);
				goto case 37;
			}
			case 536: {
				PopContext();
				goto case 23;
			}
			case 537: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 538;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				Expect(115, la); // "Enum"
				currentState = 539;
				break;
			}
			case 539: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(540);
				goto case 205;
			}
			case 540: {
				PopContext();
				goto case 541;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 63) {
					currentState = 553;
					break;
				} else {
					goto case 542;
				}
			}
			case 542: {
				stateStack.Push(543);
				goto case 23;
			}
			case 543: {
				SetIdentifierExpected(la);
				goto case 544;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (set[97].Get(la.kind)) {
					goto case 548;
				} else {
					Expect(113, la); // "End"
					currentState = 545;
					break;
				}
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				Expect(115, la); // "Enum"
				currentState = 546;
				break;
			}
			case 546: {
				stateStack.Push(547);
				goto case 23;
			}
			case 547: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 548: {
				SetIdentifierExpected(la);
				goto case 549;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (la.kind == 40) {
					stateStack.Push(548);
					goto case 445;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(550);
					goto case 205;
				}
			}
			case 550: {
				PopContext();
				goto case 551;
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (la.kind == 20) {
					currentState = 552;
					break;
				} else {
					goto case 542;
				}
			}
			case 552: {
				stateStack.Push(542);
				goto case 55;
			}
			case 553: {
				PushContext(Context.Type, la, t);
				stateStack.Push(554);
				goto case 37;
			}
			case 554: {
				PopContext();
				goto case 542;
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				Expect(103, la); // "Delegate"
				currentState = 556;
				break;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (la.kind == 210) {
					currentState = 557;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 557;
						break;
					} else {
						Error(la);
						goto case 557;
					}
				}
			}
			case 557: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 558;
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				currentState = 559;
				break;
			}
			case 559: {
				PopContext();
				goto case 560;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				if (la.kind == 37) {
					currentState = 563;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 561;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 561: {
				PushContext(Context.Type, la, t);
				stateStack.Push(562);
				goto case 37;
			}
			case 562: {
				PopContext();
				goto case 23;
			}
			case 563: {
				SetIdentifierExpected(la);
				goto case 564;
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				if (set[150].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 566;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(565);
							goto case 430;
						} else {
							Error(la);
							goto case 565;
						}
					}
				} else {
					goto case 565;
				}
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(38, la); // ")"
				currentState = 560;
				break;
			}
			case 566: {
				stateStack.Push(565);
				goto case 504;
			}
			case 567: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 568;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (la.kind == 155) {
					currentState = 569;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 569;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 569;
							break;
						} else {
							Error(la);
							goto case 569;
						}
					}
				}
			}
			case 569: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(570);
				goto case 205;
			}
			case 570: {
				PopContext();
				goto case 571;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				if (la.kind == 37) {
					currentState = 721;
					break;
				} else {
					goto case 572;
				}
			}
			case 572: {
				stateStack.Push(573);
				goto case 23;
			}
			case 573: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 574;
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 718;
				} else {
					goto case 575;
				}
			}
			case 575: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 576;
			}
			case 576: {
				if (la == null) { currentState = 576; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 712;
				} else {
					goto case 577;
				}
			}
			case 577: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 578;
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (set[101].Get(la.kind)) {
					goto case 583;
				} else {
					isMissingModifier = false;
					goto case 579;
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				Expect(113, la); // "End"
				currentState = 580;
				break;
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 155) {
					currentState = 581;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 581;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 581;
							break;
						} else {
							Error(la);
							goto case 581;
						}
					}
				}
			}
			case 581: {
				stateStack.Push(582);
				goto case 23;
			}
			case 582: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 583: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 584;
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				if (la.kind == 40) {
					stateStack.Push(583);
					goto case 445;
				} else {
					isMissingModifier = true;
					goto case 585;
				}
			}
			case 585: {
				SetIdentifierExpected(la);
				goto case 586;
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				if (set[132].Get(la.kind)) {
					currentState = 711;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 587;
				}
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(577);
					goto case 567;
				} else {
					if (la.kind == 103) {
						stateStack.Push(577);
						goto case 555;
					} else {
						if (la.kind == 115) {
							stateStack.Push(577);
							goto case 537;
						} else {
							if (la.kind == 142) {
								stateStack.Push(577);
								goto case 9;
							} else {
								if (set[104].Get(la.kind)) {
									stateStack.Push(577);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 588;
								} else {
									Error(la);
									goto case 577;
								}
							}
						}
					}
				}
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				if (set[122].Get(la.kind)) {
					stateStack.Push(589);
					goto case 696;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(589);
						goto case 678;
					} else {
						if (la.kind == 101) {
							stateStack.Push(589);
							goto case 665;
						} else {
							if (la.kind == 119) {
								stateStack.Push(589);
								goto case 653;
							} else {
								if (la.kind == 98) {
									stateStack.Push(589);
									goto case 641;
								} else {
									if (la.kind == 186) {
										stateStack.Push(589);
										goto case 604;
									} else {
										if (la.kind == 172) {
											stateStack.Push(589);
											goto case 590;
										} else {
											Error(la);
											goto case 589;
										}
									}
								}
							}
						}
					}
				}
			}
			case 589: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				Expect(172, la); // "Operator"
				currentState = 591;
				break;
			}
			case 591: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 592;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				currentState = 593;
				break;
			}
			case 593: {
				PopContext();
				goto case 594;
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				Expect(37, la); // "("
				currentState = 595;
				break;
			}
			case 595: {
				stateStack.Push(596);
				goto case 430;
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				Expect(38, la); // ")"
				currentState = 597;
				break;
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				if (la.kind == 63) {
					currentState = 601;
					break;
				} else {
					goto case 598;
				}
			}
			case 598: {
				stateStack.Push(599);
				goto case 261;
			}
			case 599: {
				if (la == null) { currentState = 599; break; }
				Expect(113, la); // "End"
				currentState = 600;
				break;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 601: {
				PushContext(Context.Type, la, t);
				goto case 602;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (la.kind == 40) {
					stateStack.Push(602);
					goto case 445;
				} else {
					stateStack.Push(603);
					goto case 37;
				}
			}
			case 603: {
				PopContext();
				goto case 598;
			}
			case 604: {
				if (la == null) { currentState = 604; break; }
				Expect(186, la); // "Property"
				currentState = 605;
				break;
			}
			case 605: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(606);
				goto case 205;
			}
			case 606: {
				PopContext();
				goto case 607;
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				if (la.kind == 37) {
					stateStack.Push(608);
					goto case 426;
				} else {
					goto case 608;
				}
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				if (la.kind == 63) {
					currentState = 638;
					break;
				} else {
					goto case 609;
				}
			}
			case 609: {
				if (la == null) { currentState = 609; break; }
				if (la.kind == 136) {
					currentState = 633;
					break;
				} else {
					goto case 610;
				}
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				if (la.kind == 20) {
					currentState = 632;
					break;
				} else {
					goto case 611;
				}
			}
			case 611: {
				stateStack.Push(612);
				goto case 23;
			}
			case 612: {
				PopContext();
				goto case 613;
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				if (la.kind == 40) {
					stateStack.Push(613);
					goto case 445;
				} else {
					goto case 614;
				}
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (set[151].Get(la.kind)) {
					currentState = 631;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 615;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				if (la.kind == 128) {
					currentState = 616;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 616;
						break;
					} else {
						Error(la);
						goto case 616;
					}
				}
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				if (la.kind == 37) {
					stateStack.Push(617);
					goto case 426;
				} else {
					goto case 617;
				}
			}
			case 617: {
				stateStack.Push(618);
				goto case 261;
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				Expect(113, la); // "End"
				currentState = 619;
				break;
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				if (la.kind == 128) {
					currentState = 620;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 620;
						break;
					} else {
						Error(la);
						goto case 620;
					}
				}
			}
			case 620: {
				stateStack.Push(621);
				goto case 23;
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				if (set[110].Get(la.kind)) {
					goto case 624;
				} else {
					goto case 622;
				}
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(113, la); // "End"
				currentState = 623;
				break;
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				if (la.kind == 40) {
					stateStack.Push(624);
					goto case 445;
				} else {
					goto case 625;
				}
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (set[151].Get(la.kind)) {
					currentState = 625;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 626;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 626;
							break;
						} else {
							Error(la);
							goto case 626;
						}
					}
				}
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				if (la.kind == 37) {
					stateStack.Push(627);
					goto case 426;
				} else {
					goto case 627;
				}
			}
			case 627: {
				stateStack.Push(628);
				goto case 261;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				Expect(113, la); // "End"
				currentState = 629;
				break;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				if (la.kind == 128) {
					currentState = 630;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 630;
						break;
					} else {
						Error(la);
						goto case 630;
					}
				}
			}
			case 630: {
				stateStack.Push(622);
				goto case 23;
			}
			case 631: {
				SetIdentifierExpected(la);
				goto case 614;
			}
			case 632: {
				stateStack.Push(611);
				goto case 55;
			}
			case 633: {
				PushContext(Context.Type, la, t);
				stateStack.Push(634);
				goto case 37;
			}
			case 634: {
				PopContext();
				goto case 635;
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				if (la.kind == 22) {
					currentState = 636;
					break;
				} else {
					goto case 610;
				}
			}
			case 636: {
				PushContext(Context.Type, la, t);
				stateStack.Push(637);
				goto case 37;
			}
			case 637: {
				PopContext();
				goto case 635;
			}
			case 638: {
				PushContext(Context.Type, la, t);
				goto case 639;
			}
			case 639: {
				if (la == null) { currentState = 639; break; }
				if (la.kind == 40) {
					stateStack.Push(639);
					goto case 445;
				} else {
					if (la.kind == 162) {
						stateStack.Push(640);
						goto case 85;
					} else {
						if (set[16].Get(la.kind)) {
							stateStack.Push(640);
							goto case 37;
						} else {
							Error(la);
							goto case 640;
						}
					}
				}
			}
			case 640: {
				PopContext();
				goto case 609;
			}
			case 641: {
				if (la == null) { currentState = 641; break; }
				Expect(98, la); // "Custom"
				currentState = 642;
				break;
			}
			case 642: {
				stateStack.Push(643);
				goto case 653;
			}
			case 643: {
				if (la == null) { currentState = 643; break; }
				if (set[115].Get(la.kind)) {
					goto case 645;
				} else {
					Expect(113, la); // "End"
					currentState = 644;
					break;
				}
			}
			case 644: {
				if (la == null) { currentState = 644; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				if (la.kind == 40) {
					stateStack.Push(645);
					goto case 445;
				} else {
					if (la.kind == 56) {
						currentState = 646;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 646;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 646;
								break;
							} else {
								Error(la);
								goto case 646;
							}
						}
					}
				}
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				Expect(37, la); // "("
				currentState = 647;
				break;
			}
			case 647: {
				stateStack.Push(648);
				goto case 430;
			}
			case 648: {
				if (la == null) { currentState = 648; break; }
				Expect(38, la); // ")"
				currentState = 649;
				break;
			}
			case 649: {
				stateStack.Push(650);
				goto case 261;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				Expect(113, la); // "End"
				currentState = 651;
				break;
			}
			case 651: {
				if (la == null) { currentState = 651; break; }
				if (la.kind == 56) {
					currentState = 652;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 652;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 652;
							break;
						} else {
							Error(la);
							goto case 652;
						}
					}
				}
			}
			case 652: {
				stateStack.Push(643);
				goto case 23;
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				Expect(119, la); // "Event"
				currentState = 654;
				break;
			}
			case 654: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(655);
				goto case 205;
			}
			case 655: {
				PopContext();
				goto case 656;
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				if (la.kind == 63) {
					currentState = 663;
					break;
				} else {
					if (set[152].Get(la.kind)) {
						if (la.kind == 37) {
							stateStack.Push(657);
							goto case 426;
						} else {
							goto case 657;
						}
					} else {
						Error(la);
						goto case 657;
					}
				}
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				if (la.kind == 136) {
					currentState = 658;
					break;
				} else {
					goto case 23;
				}
			}
			case 658: {
				PushContext(Context.Type, la, t);
				stateStack.Push(659);
				goto case 37;
			}
			case 659: {
				PopContext();
				goto case 660;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				if (la.kind == 22) {
					currentState = 661;
					break;
				} else {
					goto case 23;
				}
			}
			case 661: {
				PushContext(Context.Type, la, t);
				stateStack.Push(662);
				goto case 37;
			}
			case 662: {
				PopContext();
				goto case 660;
			}
			case 663: {
				PushContext(Context.Type, la, t);
				stateStack.Push(664);
				goto case 37;
			}
			case 664: {
				PopContext();
				goto case 657;
			}
			case 665: {
				if (la == null) { currentState = 665; break; }
				Expect(101, la); // "Declare"
				currentState = 666;
				break;
			}
			case 666: {
				if (la == null) { currentState = 666; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 667;
					break;
				} else {
					goto case 667;
				}
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				if (la.kind == 210) {
					currentState = 668;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 668;
						break;
					} else {
						Error(la);
						goto case 668;
					}
				}
			}
			case 668: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(669);
				goto case 205;
			}
			case 669: {
				PopContext();
				goto case 670;
			}
			case 670: {
				if (la == null) { currentState = 670; break; }
				Expect(149, la); // "Lib"
				currentState = 671;
				break;
			}
			case 671: {
				if (la == null) { currentState = 671; break; }
				Expect(3, la); // LiteralString
				currentState = 672;
				break;
			}
			case 672: {
				if (la == null) { currentState = 672; break; }
				if (la.kind == 59) {
					currentState = 677;
					break;
				} else {
					goto case 673;
				}
			}
			case 673: {
				if (la == null) { currentState = 673; break; }
				if (la.kind == 37) {
					stateStack.Push(674);
					goto case 426;
				} else {
					goto case 674;
				}
			}
			case 674: {
				if (la == null) { currentState = 674; break; }
				if (la.kind == 63) {
					currentState = 675;
					break;
				} else {
					goto case 23;
				}
			}
			case 675: {
				PushContext(Context.Type, la, t);
				stateStack.Push(676);
				goto case 37;
			}
			case 676: {
				PopContext();
				goto case 23;
			}
			case 677: {
				if (la == null) { currentState = 677; break; }
				Expect(3, la); // LiteralString
				currentState = 673;
				break;
			}
			case 678: {
				if (la == null) { currentState = 678; break; }
				if (la.kind == 210) {
					currentState = 679;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 679;
						break;
					} else {
						Error(la);
						goto case 679;
					}
				}
			}
			case 679: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 680;
			}
			case 680: {
				if (la == null) { currentState = 680; break; }
				currentState = 681;
				break;
			}
			case 681: {
				PopContext();
				goto case 682;
			}
			case 682: {
				if (la == null) { currentState = 682; break; }
				if (la.kind == 37) {
					currentState = 692;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 690;
						break;
					} else {
						goto case 683;
					}
				}
			}
			case 683: {
				if (la == null) { currentState = 683; break; }
				if (la.kind == 134 || la.kind == 136) {
					currentState = 687;
					break;
				} else {
					goto case 684;
				}
			}
			case 684: {
				stateStack.Push(685);
				goto case 261;
			}
			case 685: {
				if (la == null) { currentState = 685; break; }
				Expect(113, la); // "End"
				currentState = 686;
				break;
			}
			case 686: {
				if (la == null) { currentState = 686; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 534;
					}
				}
			}
			case 687: {
				if (la == null) { currentState = 687; break; }
				if (la.kind == 153 || la.kind == 158 || la.kind == 159) {
					currentState = 689;
					break;
				} else {
					goto case 688;
				}
			}
			case 688: {
				stateStack.Push(684);
				goto case 37;
			}
			case 689: {
				if (la == null) { currentState = 689; break; }
				Expect(26, la); // "."
				currentState = 688;
				break;
			}
			case 690: {
				PushContext(Context.Type, la, t);
				stateStack.Push(691);
				goto case 37;
			}
			case 691: {
				PopContext();
				goto case 683;
			}
			case 692: {
				SetIdentifierExpected(la);
				goto case 693;
			}
			case 693: {
				if (la == null) { currentState = 693; break; }
				if (set[150].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 695;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(694);
							goto case 430;
						} else {
							Error(la);
							goto case 694;
						}
					}
				} else {
					goto case 694;
				}
			}
			case 694: {
				if (la == null) { currentState = 694; break; }
				Expect(38, la); // ")"
				currentState = 682;
				break;
			}
			case 695: {
				stateStack.Push(694);
				goto case 504;
			}
			case 696: {
				stateStack.Push(697);
				SetIdentifierExpected(la);
				goto case 698;
			}
			case 697: {
				if (la == null) { currentState = 697; break; }
				if (la.kind == 22) {
					currentState = 696;
					break;
				} else {
					goto case 23;
				}
			}
			case 698: {
				if (la == null) { currentState = 698; break; }
				if (la.kind == 88) {
					currentState = 699;
					break;
				} else {
					goto case 699;
				}
			}
			case 699: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(700);
				goto case 710;
			}
			case 700: {
				PopContext();
				goto case 701;
			}
			case 701: {
				if (la == null) { currentState = 701; break; }
				if (la.kind == 33) {
					currentState = 702;
					break;
				} else {
					goto case 702;
				}
			}
			case 702: {
				if (la == null) { currentState = 702; break; }
				if (la.kind == 37) {
					currentState = 707;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 704;
						break;
					} else {
						goto case 703;
					}
				}
			}
			case 703: {
				if (la == null) { currentState = 703; break; }
				if (la.kind == 20) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 704: {
				PushContext(Context.Type, la, t);
				goto case 705;
			}
			case 705: {
				if (la == null) { currentState = 705; break; }
				if (la.kind == 162) {
					stateStack.Push(706);
					goto case 85;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(706);
						goto case 37;
					} else {
						Error(la);
						goto case 706;
					}
				}
			}
			case 706: {
				PopContext();
				goto case 703;
			}
			case 707: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 708;
			}
			case 708: {
				if (la == null) { currentState = 708; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(709);
					goto case 55;
				} else {
					goto case 709;
				}
			}
			case 709: {
				if (la == null) { currentState = 709; break; }
				if (la.kind == 22) {
					currentState = 707;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 702;
					break;
				}
			}
			case 710: {
				if (la == null) { currentState = 710; break; }
				if (set[137].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 144;
					} else {
						if (la.kind == 126) {
							goto case 128;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 711: {
				isMissingModifier = false;
				goto case 585;
			}
			case 712: {
				if (la == null) { currentState = 712; break; }
				Expect(136, la); // "Implements"
				currentState = 713;
				break;
			}
			case 713: {
				PushContext(Context.Type, la, t);
				stateStack.Push(714);
				goto case 37;
			}
			case 714: {
				PopContext();
				goto case 715;
			}
			case 715: {
				if (la == null) { currentState = 715; break; }
				if (la.kind == 22) {
					currentState = 716;
					break;
				} else {
					stateStack.Push(577);
					goto case 23;
				}
			}
			case 716: {
				PushContext(Context.Type, la, t);
				stateStack.Push(717);
				goto case 37;
			}
			case 717: {
				PopContext();
				goto case 715;
			}
			case 718: {
				if (la == null) { currentState = 718; break; }
				Expect(140, la); // "Inherits"
				currentState = 719;
				break;
			}
			case 719: {
				PushContext(Context.Type, la, t);
				stateStack.Push(720);
				goto case 37;
			}
			case 720: {
				PopContext();
				stateStack.Push(575);
				goto case 23;
			}
			case 721: {
				if (la == null) { currentState = 721; break; }
				Expect(169, la); // "Of"
				currentState = 722;
				break;
			}
			case 722: {
				stateStack.Push(723);
				goto case 504;
			}
			case 723: {
				if (la == null) { currentState = 723; break; }
				Expect(38, la); // ")"
				currentState = 572;
				break;
			}
			case 724: {
				isMissingModifier = false;
				goto case 28;
			}
			case 725: {
				PushContext(Context.Type, la, t);
				stateStack.Push(726);
				goto case 37;
			}
			case 726: {
				PopContext();
				goto case 727;
			}
			case 727: {
				if (la == null) { currentState = 727; break; }
				if (la.kind == 22) {
					currentState = 728;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 728: {
				PushContext(Context.Type, la, t);
				stateStack.Push(729);
				goto case 37;
			}
			case 729: {
				PopContext();
				goto case 727;
			}
			case 730: {
				if (la == null) { currentState = 730; break; }
				Expect(169, la); // "Of"
				currentState = 731;
				break;
			}
			case 731: {
				stateStack.Push(732);
				goto case 504;
			}
			case 732: {
				if (la == null) { currentState = 732; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 733: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 734;
			}
			case 734: {
				if (la == null) { currentState = 734; break; }
				if (set[49].Get(la.kind)) {
					currentState = 734;
					break;
				} else {
					PopContext();
					stateStack.Push(735);
					goto case 23;
				}
			}
			case 735: {
				if (la == null) { currentState = 735; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(735);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 736;
					break;
				}
			}
			case 736: {
				if (la == null) { currentState = 736; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 737: {
				if (la == null) { currentState = 737; break; }
				Expect(137, la); // "Imports"
				currentState = 738;
				break;
			}
			case 738: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 739;
			}
			case 739: {
				if (la == null) { currentState = 739; break; }
				if (set[153].Get(la.kind)) {
					currentState = 745;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 741;
						break;
					} else {
						Error(la);
						goto case 740;
					}
				}
			}
			case 740: {
				PopContext();
				goto case 23;
			}
			case 741: {
				stateStack.Push(742);
				goto case 205;
			}
			case 742: {
				if (la == null) { currentState = 742; break; }
				Expect(20, la); // "="
				currentState = 743;
				break;
			}
			case 743: {
				if (la == null) { currentState = 743; break; }
				Expect(3, la); // LiteralString
				currentState = 744;
				break;
			}
			case 744: {
				if (la == null) { currentState = 744; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 740;
				break;
			}
			case 745: {
				if (la == null) { currentState = 745; break; }
				if (la.kind == 33 || la.kind == 37) {
					stateStack.Push(745);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 746;
						break;
					} else {
						goto case 740;
					}
				}
			}
			case 746: {
				stateStack.Push(740);
				goto case 37;
			}
			case 747: {
				if (la == null) { currentState = 747; break; }
				Expect(173, la); // "Option"
				currentState = 748;
				break;
			}
			case 748: {
				if (la == null) { currentState = 748; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 750;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 749;
						break;
					} else {
						goto case 534;
					}
				}
			}
			case 749: {
				if (la == null) { currentState = 749; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 534;
					}
				}
			}
			case 750: {
				if (la == null) { currentState = 750; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 23;
					break;
				} else {
					goto case 23;
				}
			}
		}

		if (la != null) {
			t = la;
			nextTokenIsPotentialStartOfExpression = false;
			readXmlIdentifier = false;
			nextTokenIsStartOfImportsOrAccessExpression = false;
			wasQualifierTokenAtStart = false;
			identifierExpected = false;
		}
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	public BitArray GetExpectedSet() { return GetExpectedSet(currentState); }
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {1, 256, 1048576, 537395328, 402670080, 444604481, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 402670080, 444596289, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 402669568, 444596289, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 402669568, 444596289, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 402669568, 444596288, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 537395328, 402669568, 444596288, 131200, 0}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671109120, 1589117122, 393600, 3328}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671105024, 1589117122, 393600, 3328}),
		new BitArray(new int[] {5, 1140850944, 26214479, -493220892, 940361760, 1606227139, -2143942272, 3393}),
		new BitArray(new int[] {0, 256, 1048576, -1601699136, 671105024, 1589117122, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -1601699136, 671105024, 1589117122, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564478, 889192447, 65, 1074825472, 72844640, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, 889192413, 65, 1074825472, 72844640, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106740, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-66189316, 1174405176, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1007673342, 889192413, 65, 1074825472, 72843552, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1038334, -1258291211, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972018405, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {-1040382, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {0, 0, -60035072, 1027, 0, 0, 134217728, 0}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1310752, 65536, 1050656, 64}),
		new BitArray(new int[] {4194304, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018401, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {4194304, 67108864, 64, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {66189314, -1174405161, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
		new BitArray(new int[] {65140738, 973078487, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8387}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-64092162, -973078488, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-64092162, 1191182376, -1048865, -546062565, -1014191950, -1593504452, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-66189316, 1174405162, -51384097, -972018401, -1030969178, 17106228, -97186288, 8259}),
		new BitArray(new int[] {6291458, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1174405160, -51384097, -971985637, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {2097158, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 97}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 32}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105928, -2144335872, 65}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969166, 17106228, -97186284, 8259}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972002021, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 129}),
		new BitArray(new int[] {2097154, 0, 0, 32768, 0, 0, 0, 129}),
		new BitArray(new int[] {-66189316, 1174405160, -51383073, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -971985637, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -972018405, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680476901, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {2097154, 32, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483614, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {5242880, -2147483550, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {5242880, -2147483552, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 143435812, 17106176, -533656048, 67}),
		new BitArray(new int[] {7, 1157628162, 26477055, -493212676, 948758565, 2147308999, -533262382, 3395}),
		new BitArray(new int[] {918528, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-318462, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-383998, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850754, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850698, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850690, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108478212, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671930656, 1606227138, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926560, 1606227138, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926304, 1606227138, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493351964, 671926304, 1606227138, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -493351964, 671926304, 1606227138, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -1030223452, 135055392, 84218880, -2143942656, 65}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 84218880, -2144073728, 65}),
		new BitArray(new int[] {3145730, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926305, 1606227138, -2143942208, 3393}),
		new BitArray(new int[] {0, 256, 0, 537001984, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 256, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {7340034, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 320, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 320, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, -1030969308, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {7340034, -2147483614, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537526400, 402669568, 444596289, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 34, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 268435456, 444596288, 128, 0}),
		new BitArray(new int[] {0, 0, 0, 536871488, 536870912, 1522008258, 384, 3328}),
		new BitArray(new int[] {0, 0, 262288, 8216, 8396800, 256, 1610679824, 2}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1016, 0, 0, 67108864, -1040187392, 32, 33554432, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 0, -1133776896, 3, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680378597, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {-66189316, 1174405160, -51383585, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 65536, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 288, 0, 0, 4210688, 0, 0}),
		new BitArray(new int[] {-18434, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-22530, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-32770, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-37890, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2050, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-6146, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 0, 0}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67})

	};

} // end Parser


}