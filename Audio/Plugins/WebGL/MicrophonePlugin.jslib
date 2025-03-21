var MicrophonePlugin = {

	buffer: undefined,

	Init: function () {

		console.log("Init:");

		// START - used to read the volume
		document.volume = 0;
		var byteOffset = 0;
		var length = 1024;
		this.buffer = new ArrayBuffer(4 * length);
		document.dataArray = new Float32Array(this.buffer, byteOffset, length);
		// END - used to read the volume

		navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;

		if (navigator.getUserMedia) {

			var constraints = {
				audio: {
					optional: [{
						sourceId: "audioSource"
					}]
				}
			};
			navigator.getUserMedia = (navigator.getUserMedia ||
				navigator.webkitGetUserMedia ||
				navigator.mozGetUserMedia ||
				navigator.msGetUserMedia);
			navigator.getUserMedia(constraints, function (stream) {
				console.log('navigator.getUserMedia successCallback: ', stream);

				document.position = 0;

				document.audioContext = new AudioContext();
				document.tempSize = 1024;
				document.tempArray = new Float32Array(document.tempSize)
				document.analyser = document.audioContext.createAnalyser();
				document.analyser.minDecibels = -90;
				document.analyser.maxDecibels = -10;
				document.analyser.smoothingTimeConstant = 0.85;

				document.mediaRecorder = new MediaRecorder(stream);

				document.source = document.audioContext.createMediaStreamSource(stream);

				document.source.connect(document.analyser);

				document.mediaRecorder.start();
				console.log(document.mediaRecorder.state);

				document.readDataOnInterval = function () {

					if (document.dataArray == undefined) {
						setTimeout(document.readDataOnInterval, 250); //wait to be set
						return;
					}

					document.tempInterval = Math.floor(document.tempSize / document.dataArray.length * 250);

					// read the next chunk after interval
					setTimeout(document.readDataOnInterval, document.tempInterval); //if mic is still active

					if (document.dataArray == undefined) {
						return;
					}

					//read the temp data buffer
					document.analyser.getFloatTimeDomainData(document.tempArray);

					// use the amplitude to get volume
					document.volume = 0;

					var j = (document.position + document.dataArray.length - document.tempSize) % document.dataArray.length;
					for (var i = 0; i < document.tempSize; ++i) {
						document.volume = Math.max(document.volume, Math.abs(document.tempArray[i]));
						document.dataArray[j] = document.tempArray[i];
						j = (j + 1) % document.dataArray.length;
					}
					document.position = (document.position + document.tempSize) % document.dataArray.length;

				};

				document.readDataOnInterval();


			}, function (error) {
				console.error('navigator.getUserMedia errorCallback: ', error);
			});
		}
	},

	QueryAudioInput: function () {

		console.log("QueryAudioInput");

		document.mMicrophones = [];

		if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
			console.log("enumerateDevices() not supported.");
		} else {
			// List microphones
			navigator.mediaDevices.enumerateDevices()
				.then(function (devices) {
					devices.forEach(function (device) {
						console.log("QueryAudioInput: kind=" + device.kind + " device=", device, " label=" + device.label);
						if (device.kind === 'audioinput') {
							document.mMicrophones.push(device.label);
						}
					});
				})
				.catch(function (err) {
					console.error(err.name + ": " + err.message);
				});
		}
	},

	GetNumberOfMicrophones: function () {
		console.log("GetNumberOfMicrophones");
		var microphones = document.mMicrophones;
		if (microphones == undefined) {
			console.log("GetNumberOfMicrophones", 0);
			return 0;
		}
		console.log("GetNumberOfMicrophones length=" + microphones.length);
		return microphones.length;
	},

	GetMicrophoneDeviceName: function (index) {
		//console.log("GetMicrophoneDeviceName");
		var returnStr = "Not Set";
		var microphones = document.mMicrophones;
		if (microphones != undefined) {
			if (index >= 0 && index < microphones.length) {
				if (microphones[index] != undefined) {
					returnStr = microphones[index];
				}
			}
		}
		console.log("GetMicrophoneDeviceName", returnStr);
		var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
		writeStringToMemory(returnStr, buffer);
		return buffer;
	},

	GetMicrophoneVolume: function (index) {
		console.log("GetMicrophoneVolume");
		if (document.volume == undefined) {
			return 0;
		}
		console.log("GetMicrophoneVolume", document.volume);
		return document.volume;
	},

	Start: function (deviceName, loop, duration, sampleRate) {
		console.log("Starting Microphone:", deviceName, "Loop:", loop, "Duration:", duration, "SampleRate:", sampleRate);

		var constraints = { audio: { deviceId: deviceName ? { exact: deviceName } : undefined } };
		navigator.mediaDevices.getUserMedia(constraints)
			.then(function (stream) {
				document.audioContext = new AudioContext({ sampleRate: sampleRate });

				document.source = document.audioContext.createMediaStreamSource(stream);
				document.analyser = document.audioContext.createAnalyser();
				document.mediaRecorder = new MediaRecorder(stream);

				document.source.connect(document.analyser);
				document.mediaRecorder.start();

				// Setup event to handle data when available
				document.mediaRecorder.ondataavailable = function (e) {
					var reader = new FileReader();
					reader.onload = function (event) {
						var data = event.target.result;
						// Call Unity function to handle the data
						SendMessage('WebGLMicrophone', 'HandleAudioData', data);
					};
					reader.readAsDataURL(e.data);
				};

				console.log("Recording started with state:", document.mediaRecorder.state);
			})
			.catch(function (error) {
				console.error("Failed to start recording:", error);
			});
	},

	GetPosition: function (deviceName) {
		// change mic device to the one specified if it is not the same and deviceName is not null or empty
		if (deviceName && deviceName !== document.deviceName) {
			document.deviceName = deviceName;
			if (document.source) {
				document.source.disconnect();
			}
			var constraints = { audio: { deviceId: deviceName ? { exact: deviceName } : undefined } };
			navigator.mediaDevices.getUserMedia(constraints)
				.then(function (stream) {
					document.source = document.audioContext.createMediaStreamSource(stream);
					document.source.connect(document.analyser);
					document.mediaRecorder = new MediaRecorder(stream);
					document.mediaRecorder.start();
					console.log("Recording started with state:", document.mediaRecorder.state);
				})
				.catch(function (error) {
					console.error("Failed to start recording:", error);
				});
		}

		if (document.audioContext) {
			const currentTime = document.audioContext.currentTime; // 초 단위
			const sampleRate = document.audioContext.sampleRate; // 초당 샘플 수
			const currentPosition = Math.floor(currentTime * sampleRate); // 샘플 단위로 변환
			console.log('Current recording position (samples): ' + currentPosition);
			return currentPosition;
		}
		return 0;
	},
};

mergeInto(LibraryManager.library, MicrophonePlugin);
